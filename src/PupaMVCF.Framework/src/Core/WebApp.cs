using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Generators;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Framework.Core;

public abstract class WebApp : IHostedService, IWebAppContext {
   public static IWebAppContext Context { get; private set; } = null!;
   private readonly IRouter _router;
   private readonly WebApplication _host;
   private readonly ILogger<WebApp> _logger;
   private readonly IWebAppBootstrap? _bootstrap;
   public IConfiguration Configuration { get; }
   public HttpClient Client { get; }


   public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web) {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      PropertyNameCaseInsensitive = true,
      WriteIndented = false
   };

   protected WebApp(IConfiguration configuration, JwtTokenGeneratorService jwtTokenGeneratorService, IRouter router,
      ILogger<WebApp> logger,
      IWebAppBootstrap? bootstrap = null!) {
      if (Context != null)
         throw new InvalidOperationException("App provider has already been configured");
      Configuration = configuration;
      _logger = logger;
      _bootstrap = bootstrap;
      _router = router;
      Client = new HttpClient {
         Timeout = configuration.GetValue<TimeSpan>("TimeoutClient")
      };
      Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
      var builder = WebApplication.CreateBuilder();

      builder.WebHost.UseKestrel(options => {
         options.Configure(configuration.GetSection("Kestrel"));
         options.Listen(IPAddress.Parse(Configuration.GetValue<string>("Ip") ?? throw new Exception("Undefined Ip.")),
            Configuration.GetValue<int>("Port"),
            listenOptions => {
               if (Configuration.GetValue<bool>("HttpsEnable"))
                  listenOptions.UseHttps();
            });
      });

      builder.Services.AddDistributedMemoryCache();
      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(jwtTokenGeneratorService.JwtSecretBytes),
               ValidateIssuer = false,
               ValidateAudience = false,
               ClockSkew = TimeSpan.Zero
            };
         });

      _host = builder.Build();
      _host.UseAuthentication();
      _host.Use(async (HttpContext context, RequestDelegate _) => {
         try {
            var request = new Request(context.Request);
            var response = new Response(context.Response);
            await _router.Execute(request, response, context.RequestAborted);
            await response.SendAsync(context.RequestAborted);
         } catch (Exception ex) {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(ex.ToString(), context.RequestAborted);
         }
      });

      Context = this;
   }

   public async Task StartAsync(CancellationToken cancellationToken) {
      _logger.LogInformation(_router.ToString());
      _logger.LogInformation(
         "🍊 PupaMVCF [Kestrel] server starting on http://{Ip}:{Port}/",
         Configuration.GetValue<string>("Ip"),
         Configuration.GetValue<int>("Port"));
      if (_bootstrap is not null) {
         _logger.LogInformation($"<< BOOTSTRAP LOADER >>");
         var operations = _bootstrap.Operations();
         
         for (var i = 1; operations.Count > 0; i++) {
            _logger.LogInformation("⚡ Execute operation number [{I}]", i);
            var op = operations.Dequeue();
            await op();
         }

         _logger.LogInformation("🏁 Complete bootstrap loader");
      }

      await _host.StartAsync(cancellationToken);
   }

   public async Task StopAsync(CancellationToken cancellationToken) {
      _logger.LogInformation("🍊 PupaMVCF [Kestrel] server stopping...");
      await _host.StopAsync(cancellationToken);
   }

   public async Task Dispose() {
      Client?.Dispose();
      await _host.DisposeAsync();
      Context = null!;
   }
}