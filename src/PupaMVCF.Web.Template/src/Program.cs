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
      var builder = Host.CreateApplicationBuilder(args);
      builder.Services.AddSingleton<IValidatorManager, ModifyValidatorManager>(_ =>
         new ModifyValidatorManager(builder.Configuration,
         [
            new NeedValidatorModule(), new EmailValidatorModule(), new NumberRangeValidatorModule(),
            new StringRangeValidatorModule(), new CloudflareCaptchaValidatorModule()
         ]));
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware(), new TemplateMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController(), new TemplateController(), new ErrorControllerOnlyJson()
         ]);
         return new Router(routerMapBuilder);
      });
      builder.Services.AddHostedService<TemplateApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}