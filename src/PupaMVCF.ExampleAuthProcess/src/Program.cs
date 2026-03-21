using Grpc.Core;

using Protos.Auth;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Database.PgSql;
using PupaMVCF.ExampleAuthProcess.Services;

namespace PupaMVCF.ExampleAuthProcess;

public static class Program {
   private static async Task Main(string[] args) {
      var builder = Host.CreateApplicationBuilder(args);
      builder.Configuration.AddJsonFile("secrets.json");
      builder.Services.AddSingleton<IEnumerable<ServerServiceDefinition>>(_ =>
         [AuthService.BindService(new AuthGrpcService())]);
      builder.Services.AddSingleton<IDatabaseProcessor, DatabasePgSqlProcessor>();
      builder.Services.AddSingleton<ExampleAuthServiceApp>();
      builder.Services.AddHostedService<ExampleAuthProcessWorker>();
      var host = builder.Build();
      await host.RunAsync();
   }
}