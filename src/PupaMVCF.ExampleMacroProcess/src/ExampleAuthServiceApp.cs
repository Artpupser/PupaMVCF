using Grpc.Core;

using PupaMVCF.Framework.Macro;

namespace PupaMVCF.ExampleMacroProcess;

public sealed class ExampleAuthServiceApp : MacroApp {
   public ExampleAuthServiceApp(IConfiguration configuration,
      IEnumerable<ServerServiceDefinition> services,
      ILogger<MacroApp> logger) : base(configuration, services, logger) { }
}