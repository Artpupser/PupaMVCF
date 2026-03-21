using Grpc.Core;

using PupaMVCF.Framework;
using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Macro;

namespace PupaMVCF.ExampleAuthProcess;

public sealed class ExampleAuthServiceApp : MacroApp {
   public ExampleAuthServiceApp(IConfiguration configuration,
      IEnumerable<ServerServiceDefinition> services, IDatabaseProcessor databaseProcessor,
      ILogger<BaseApp> logger) : base(configuration, databaseProcessor, services, logger) { }
}