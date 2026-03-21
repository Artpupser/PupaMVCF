using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.Framework.Core;

public abstract class WebApp : IHostedService, ISecureWebAppProvider {
   public static ISecureWebAppProvider SecureInstance { get; private set; } = null!;
   protected readonly IRouter _router;
   private readonly IWebHost _host;

   public VirtualFolder PublicFolder { get; }
   public IDatabaseProcessor DatabaseProcessor { get; }

   public IConfiguration Configuration { get; }
   public ILogger<WebApp> Logger { get; }
   public HttpClient Client { get; }
   [ConfigurationKeyName("Host")] public static string HostName { get; private set; } = string.Empty;

   [ConfigurationKeyName("Port")] public static ushort Port { get; private set; } = 50001;

   [ConfigurationKeyName("TimeoutClient")]
   public static TimeSpan TimeoutClient { get; private set; } = TimeSpan.FromSeconds(10);

   [ConfigurationKeyName("CaptchaSecureKey")]
   public static string CaptchaSecureKey { get; private set; } = string.Empty;

   [ConfigurationKeyName("CaptchaSecureSite")]
   public static string CaptchaSecureSite { get; private set; } = string.Empty;

   protected WebApp(IConfiguration configuration, IDatabaseProcessor databaseProcessor, IRouter router,
      ILogger<WebApp> logger) {
      if (SecureInstance != null)
         throw new InvalidOperationException("App provider has already been configured");
      configuration.BindConfigurationWithClass(this);
      View.LoadCssFiles(configuration);
      Logger = logger;
      Configuration = configuration;
      DatabaseProcessor = databaseProcessor;
      PublicFolder = VirtualIo.RootFolder.GetFolderIn("public") ??
                     throw new DirectoryNotFoundException("Public folder not founded");
      _router = router;
      Client = new HttpClient {
         Timeout = TimeSpan.FromSeconds(10)
      };
      var builder = new WebHostBuilder().UseKestrel(options => {
         options.Configure(configuration.GetSection("Kestrel"));
         if (HostName == "localhost") {
            options.ListenLocalhost(Port);
            return;
         }

         options.Listen(IPAddress.Parse(HostName), Port, listenOptions => { listenOptions.UseHttps(); });
      }).ConfigureServices((host, services) => {
         services.AddDistributedMemoryCache();
         services.AddSession(options => {
            var sessionConfigurationSection = configuration.GetSection("Session");
            options.IdleTimeout =
               TimeSpan.Parse(sessionConfigurationSection["IdleTimeout"] ?? throw new NullReferenceException());
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
         });
      }).Configure(app => {
         app.UseSession();
         app.Use(async (context, _) => {
            var request = new Request(context.Request, context.Session);
            var response = new Response(context.Response);
            await _router.Execute(request, response, context.RequestAborted);
            await response.SendAsync(context.RequestAborted);
         });
      });
      _host = builder.Build();
      SecureInstance = this;
   }

   public async Task StartAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("WebApp [Kestrel] server starting on https://{HostName}:{Port}/", HostName, Port);
      await _host.StartAsync(cancellationToken);
   }

   public async Task StopAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("WebApp [Kestrel] server stopping...");
      await _host.StopAsync(cancellationToken);
   }

   public void Dispose() {
      Client?.Dispose();
      _host?.Dispose();
      SecureInstance = null!;
   }
}