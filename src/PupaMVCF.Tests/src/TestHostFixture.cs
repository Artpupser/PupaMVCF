using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Generators;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Tests;

public sealed class TestHostFixture : IAsyncLifetime {
   public IHost Host { get; private set; } = null!;
   public IConfiguration Configuration { get; private set; } = null!;
   public HttpClient Client { get; private set; } = null!;

   public async Task InitializeAsync() {
      dotenv.net.DotEnv.Load();
      var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder([]);
      builder.Configuration.AddEnvironmentVariables();
      builder.Logging.AddConsole();
      await InitializatorBuilder.Except([]);
      builder.Services.AddSingleton<PublicFolder>();
      builder.Services.AddSingleton<JwtTokenGeneratorService>();
      await InitializatorBuilder.PreloadMvcComponents(builder.Services);
      builder.Services.AddSingleton<IValidatorManager, ValidatorManager>();
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<TestApp>();
      Configuration = builder.Configuration;
      Client = new HttpClient() {
         Timeout = TimeSpan.FromSeconds(10),
      };
      Host = builder.Build();
      await Host.StartAsync();
   }

   public async Task DisposeAsync() {
      await Host.StopAsync();
      Host.Dispose();
   }
}
