using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Tests.Controllers;
using PupaMVCF.Framework.Tests.Validations.Modules;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;

namespace PupaMVCF.Framework.Tests;

public sealed class TestHostFixture : IAsyncLifetime {
   public IHost Host { get; private set; } = null!;
   public IConfiguration Configuration { get; private set; } = null!;

   public async Task InitializeAsync() {
      var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder([]);
      builder.Logging.AddConsole();
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            new NeedValidatorModule(), new EmailValidatorModule(), new NumberRangeValidatorModule(),
            new StringRangeValidatorModule(), new CloudflareCaptchaValidatorModule(), new TestValidatorModule()
         ]));
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController(), new ErrorControllerOnlyJson(), new TestController()
         ]);
         return new Router(routerMapBuilder);
      });
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