using Grpc.Core;

using Protos.Auth;

using PupaMVCF.ExampleMacroProcess.Services;

namespace PupaMVCF.ExampleMacroProcess;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Services.AddSingleton<IEnumerable<ServerServiceDefinition>>(_ =>
         [AuthService.BindService(new AuthGrpcService())]);
      builder.Services.AddHostedService<ExampleAuthServiceApp>();
      var host = builder.Build();
      await host.RunAsync();
   }
}