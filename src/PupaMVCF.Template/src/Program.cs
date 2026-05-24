using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Validations.Modules;
using PupaMVCF.Template.Controllers;
using PupaMVCF.Template.Middleware;

namespace PupaMVCF.Template;

public static class Program {
   private static async Task Main(string[] args) {
      dotenv.net.DotEnv.Load();
      var builder = Host.CreateApplicationBuilder(args);
      builder.Configuration.AddEnvironmentVariables();
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
      builder.Services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory<Npgsql.NpgsqlConnection>>();
      builder.Services.AddSingleton<JwtTokenGeneratorService>();
      builder.Services.AddSingleton<PublicFolder>();
      builder.Services.AddSingleton<IWebAppBootstrap, TemplateBootstrap>();

      builder.Services.AddScoped([typeof(LoggerMiddleware), typeof(TemplateMiddleware)]);
      builder.Services.AddScoped(
         [typeof(TemplateController), typeof(ErrorControllerOnlyJson), typeof(StaticController)]);
      builder.Services.AddSingleton<RouterMapBuilder>(_ => new RouterMapBuilder().AddControllers(
         [typeof(TemplateController), typeof(ErrorControllerOnlyJson), typeof(StaticController)]));
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<TemplateApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}