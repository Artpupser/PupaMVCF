using PupaMVCF.ExampleProcess.Components;
using PupaMVCF.ExampleProcess.Controllers;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Database.PgSql;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.ExampleProcess;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Configuration.AddJsonFile("secrets.json");
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController(), new ErrorController(), new PagesController(),
            new AuthController(builder.Configuration)
         ]);
         return new Router(routerMapBuilder);
      });
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      builder.Services.AddSingleton<IDatabaseProcessor, DatabasePgSqlProcessor>();
      builder.Services.AddHostedService<ExampleApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}