using PupaMVCF.Example.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Framework.Generators;

namespace PupaMVCF.Example;

public static class Program {
   private static async Task Main(string[] args) {
      dotenv.net.DotEnv.Load();
      var builder = Host.CreateApplicationBuilder(args);
      await InitializatorBuilder.Except([]);
      builder.Services.AddSingleton<PublicFolder>();
      builder.Services.AddSingleton<JwtTokenGeneratorService>();
      await InitializatorBuilder.PreloadMvcComponents(builder.Services);
      builder.Services.AddSingleton<IValidatorManager, ValidatorManager>();
      builder.Services.AddSingleton<IRouter, Router>();
      builder.Services.AddHostedService<ExampleApp>();
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      var host = builder.Build();
      await host.RunAsync();
   }
}
