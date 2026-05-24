using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Generators;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;
using PupaMVCF.Tests.Controllers;
using PupaMVCF.Tests.Validations.Modules;

namespace PupaMVCF.Tests;

public sealed class TestHostFixture : IAsyncLifetime {
   public IHost Host { get; private set; } = null!;
   public IConfiguration Configuration { get; private set; } = null!;

   public async Task InitializeAsync() {
      dotenv.net.DotEnv.Load();
      var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder([]);
      builder.Configuration.AddEnvironmentVariables();
      builder.Logging.AddConsole();
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            typeof(NeedValidatorModule),
            typeof(EmailValidatorModule),
            typeof(NumberRangeValidatorModule),
            typeof(StringRangeValidatorModule),
            typeof(CloudflareCaptchaValidatorModule),
            typeof(JsonValidatorModule),
            typeof(TestValidatorModule)
         ]));
      builder.Services.AddSingleton<JwtTokenGeneratorService>();
      builder.Services.AddScoped([typeof(LoggerMiddleware)]);
      builder.Services.AddScoped([typeof(TestController), typeof(ErrorControllerOnlyJson), typeof(StaticController)]);
      builder.Services.AddSingleton<RouterMapBuilder>(_ =>
         new RouterMapBuilder().AddControllers([
            typeof(TestController), typeof(ErrorControllerOnlyJson), typeof(StaticController)
         ]));
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<TestApp>();
      Configuration = builder.Configuration;
      Host = builder.Build();
      await Host.StartAsync();
   }

   public async Task DisposeAsync() {
      await Host.StopAsync();
      Host.Dispose();
   }
}