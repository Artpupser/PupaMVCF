using PupaMVCF.ExampleProcess.Components;
using PupaMVCF.ExampleProcess.Controllers;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;


namespace PupaMVCF.ExampleProcess;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            new NeedValidatorModule(), new EmailValidatorModule(), new NumberRangeValidatorModule(),
            new StringRangeValidatorModule(), new CloudflareCaptchaValidatorModule()
         ]));
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware(), new ErrorMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController(), new ErrorControllerOnlyJson(), new PagesController(),
            new FruitsController()
         ]);
         return new Router(routerMapBuilder);
      });
      builder.Services.AddHostedService<ExampleApp>();
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      var host = builder.Build();
      await host.RunAsync();
   }
}