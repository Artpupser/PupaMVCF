using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Web.Template.Controllers;
using PupaMVCF.Web.Template.Middleware;

namespace PupaMVCF.Web.Template;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware(), new TemplateMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController(), new TemplateController(), new ErrorController()
         ]);
         return new Router(routerMapBuilder);
      });
      builder.Services.AddHostedService<TemplateApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}