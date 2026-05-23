using PupaMVCF.Example.Components;
using PupaMVCF.Example.Controllers;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;


namespace PupaMVCF.Example;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            typeof(NeedValidatorModule),
            typeof(EmailValidatorModule),
            typeof(NumberRangeValidatorModule),
            typeof(StringRangeValidatorModule),
            typeof(CloudflareCaptchaValidatorModule),
            typeof(JsonValidatorModule)
         ]));
      builder.Services.AddSingleton<PublicFolder>();
      builder.Services.AddScoped([typeof(LoggerMiddleware)]);
      builder.Services.AddScoped([
         typeof(FruitsController), typeof(PagesController), typeof(ErrorControllerOnlyJson), typeof(StaticController)
      ]);
      builder.Services.AddSingleton<RouterMapBuilder>(_ => new RouterMapBuilder().AddControllers([
         typeof(FruitsController), typeof(PagesController), typeof(ErrorControllerOnlyJson), typeof(StaticController)
      ]));
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<ExampleApp>();
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      var host = builder.Build();
      await host.RunAsync();
   }
}