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
      builder.Services.AddScoped<StaticController>();
      builder.Services.AddScoped<ErrorControllerOnlyJson>();
      builder.Services.AddScoped<PagesController>();
      builder.Services.AddScoped<FruitsController>();
      builder.Services.AddScoped<LoggerMiddleware>();
      builder.Services.AddSingleton<RouterMapBuilder>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddController<StaticController>();
         routerMapBuilder.AddController<ErrorControllerOnlyJson>();
         routerMapBuilder.AddController<PagesController>();
         routerMapBuilder.AddController<FruitsController>();
         return routerMapBuilder;
      });
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<ExampleApp>();
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      var host = builder.Build();
      await host.RunAsync();
   }
}