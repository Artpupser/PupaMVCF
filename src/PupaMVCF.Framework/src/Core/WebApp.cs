using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Components;

namespace PupaMVCF.Framework.Core;

public abstract class WebApp : IHostedService, IWebAppContext {
   public static IWebAppContext Context { get; private set; } = null!;
   protected readonly IRouter _router;
   private readonly IWebHost _host;

   public VirtualFolder PublicFolder { get; }
   public IConfiguration Configuration { get; }
   public ILogger<WebApp> Logger { get; }
   public IValidatorManager Validator { get; }
   public HttpClient Client { get; }


   public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web) {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      PropertyNameCaseInsensitive = true,
      WriteIndented = false
   };


   protected WebApp(IConfiguration configuration, IRouter router,
      IValidatorManager validatorManager, ILogger<WebApp> logger) {
      if (Context != null)
         throw new InvalidOperationException("App provider has already been configured");
      configuration.BindConfigurationWithClass(this);
      Logger = logger;
      Validator = validatorManager;
      Configuration = configuration;
      PublicFolder = VirtualIo.RootFolder.GetFolderIn("public") ??
                     throw new DirectoryNotFoundException("Public folder not founded");
      _router = router;
      Client = new HttpClient {
         Timeout = configuration.GetTimeSpan("TimeoutClient")
      };
      var builder = new WebHostBuilder().UseKestrel(options => {
         options.Configure(configuration.GetSection("Kestrel"));
         options.Listen(IPAddress.Parse(Configuration.GetAny<string>("Ip")), Configuration.GetAny<int>("Port"),
            listenOptions => {
               if (Configuration.GetAny<bool>("HttpsEnable")) listenOptions.UseHttps();
            });
      }).ConfigureServices((_, services) => {
         services.AddDistributedMemoryCache();
         services.AddSession(options => {
            var sessionConfigurationSection = configuration.GetSection("Session");
            var expireTimeSpan = sessionConfigurationSection.GetTimeSpan("Expire");
            options.IdleTimeout = expireTimeSpan;
            options.Cookie.MaxAge = expireTimeSpan;
            options.Cookie.Name = sessionConfigurationSection["Name"];
            options.Cookie.HttpOnly = !Configuration.GetAny<bool>("HttpsEnable");
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = (SameSiteMode)sessionConfigurationSection.GetAny<byte>("SameSite");
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
      Context = this;
   }

   public async Task StartAsync(CancellationToken cancellationToken) {
      Logger.LogInformation(
         "WebApp [Kestrel] server starting on [http://{0}:{1}/] or [https://{2}:{3}/]",
         Configuration.GetAny<string>("Ip"),
         Configuration.GetAny<int>("Port"), Configuration.GetAny<string>("Ip"),
         Configuration.GetAny<int>("Port"));
      await _host.StartAsync(cancellationToken);
   }

   public async Task StopAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("WebApp [Kestrel] server stopping...");
      await _host.StopAsync(cancellationToken);
   }

   public void Dispose() {
      Client?.Dispose();
      _host?.Dispose();
      Context = null!;
   }
}