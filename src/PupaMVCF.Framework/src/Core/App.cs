using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Routing;


namespace PupaMVCF.Framework.Core;

public abstract class App : BaseApp, ISecureAppProvider {
   protected readonly HttpListener _listener;
   protected readonly IRouter _router;
   private CancellationTokenSource? _requestListenerCancelTokenSource;
   private Task? _requestListenerTask;
   public static ISecureAppProvider SecureInstance { get; private set; } = null!;
   private bool IsWorked => _listener.IsListening;
   public HttpClient Client { get; }

   protected App(IConfiguration configuration, IDatabaseProcessor databaseProcessor, IRouter router,
      ILogger<App> logger) : base(
      configuration, databaseProcessor, logger) {
      if (SecureInstance != null)
         throw new InvalidOperationException("App provider has already been configured");
      _router = router;
      _listener = new HttpListener();
      Client = new HttpClient {
         Timeout = TimeSpan.FromSeconds(10)
      };
      _listener.Prefixes.Add($"https://{HostName}:{Port}/");
      _listener.TimeoutManager.IdleConnection = Timeout;
      _listener.TimeoutManager.DrainEntityBody = Timeout;
      _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
      _listener.ExtendedProtectionPolicy =
         new System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy(System.Security.Authentication
            .ExtendedProtection.PolicyEnforcement.Never);
      if (OperatingSystem.IsWindows()) {
         _listener.TimeoutManager.HeaderWait = Timeout;
         _listener.TimeoutManager.EntityBody = Timeout;
      }

      SecureInstance = this;
   }

   #region BASE_APP

   public override async Task Run(CancellationToken cancellationToken) {
      if (IsWorked) return;
      try {
         _requestListenerCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         _listener.Start();
         await Task.Delay(200, cancellationToken);
         _requestListenerTask = Task.Run(() => ListenRequests(_requestListenerCancelTokenSource.Token),
            _requestListenerCancelTokenSource.Token);
         Logger.LogInformation("https server run! [https://{HostName}:{Port}/]", HostName, Port);
         await _requestListenerTask;
      } catch (OperationCanceledException e) {
         Logger.LogInformation(e, "Run canceled exception");
      } catch (Exception e) {
         Logger.LogError(e, "Error: {Message}", e.Message);
      } finally {
         await Stop(cancellationToken);
      }
   }

   public override async Task Stop(CancellationToken cancellationToken) {
      if (!IsWorked) return;
      try {
         _requestListenerCancelTokenSource?.Cancel();
         if (_requestListenerTask != null) {
            Logger.LogWarning("Request listener task not null, wait 5 seconds");
            await _requestListenerTask.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken)
               .ConfigureAwait(false);
         }

         _listener.Stop();
         _listener.Close();
         Logger.LogInformation("https server stop! {HostName}:{Port}", HostName, Port);
      } finally {
         Dispose();
      }
   }

   #endregion

   #region LOGIC

   protected async Task ListenRequests(CancellationToken cancellationToken) {
      try {
         Logger.LogInformation("RequestListener start...");
         while (!cancellationToken.IsCancellationRequested && IsWorked) {
            var contextTask = _listener.GetContextAsync();
            var completedTask = await Task.WhenAny(contextTask,
               Task.Delay(System.Threading.Timeout.InfiniteTimeSpan, cancellationToken));
            if (completedTask != contextTask) continue;
            var context = await contextTask;
            _ = ProcessRequestAsync(context, cancellationToken);
         }
      } catch (OperationCanceledException e) {
         Logger.LogInformation(e, "ListenRequests cancelled");
      } catch (Exception e) {
         Logger.LogError(e, "Error: {Message}", e.Message);
      } finally {
         Logger.LogInformation("RequestListener stop...");
      }
   }

   protected async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken cancellationToken) {
      try {
         using var linkedSourceToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         linkedSourceToken.CancelAfter(TimeSpan.FromSeconds(30)); // << ENV
         Request request = new(context.Request);
         Response response = new(context.Response);
         await _router.Execute(request, response, linkedSourceToken.Token);
         await response.SendAsync(linkedSourceToken.Token);
      } catch (OperationCanceledException e) {
         Logger.LogInformation(e, "RequestListener cancelled");
      } catch (Exception e) {
         Logger.LogError(e, "Error: {Message}", e.Message);
      }
   }

   #endregion

   #region DISPOSE

   public override void Dispose() {
      _requestListenerCancelTokenSource?.Dispose();
      _requestListenerCancelTokenSource = null!;
      _requestListenerTask = null!;
      SecureInstance = null!;
      base.Dispose();
   }

   #endregion
}