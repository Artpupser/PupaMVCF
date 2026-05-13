using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;
using PupaMVCF.Web.Template.Controllers;
using PupaMVCF.Web.Template.Middleware;

namespace PupaMVCF.Web.Template;

public static class Program {
   private static async Task Main(string[] args) {
      dotenv.net.DotEnv.Load();
      var builder = Host.CreateApplicationBuilder(args);
      builder.Configuration.AddEnvironmentVariables();
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            new NeedValidatorModule(), new EmailValidatorModule(), new NumberRangeValidatorModule(),
            new StringRangeValidatorModule(), new CloudflareCaptchaValidatorModule()
         ]));
      builder.Services.AddScoped<TemplateController>();
      builder.Services.AddScoped<LoggerMiddleware>();
      builder.Services.AddScoped<TemplateMiddleware>();
      builder.Services.AddScoped<ErrorControllerOnlyJson>();
      builder.Services.AddScoped<StaticController>();
      builder.Services.AddSingleton<RouterMapBuilder>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddController<StaticController>();
         routerMapBuilder.AddController<ErrorControllerOnlyJson>();
         return new RouterMapBuilder();
      });
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<TemplateApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}