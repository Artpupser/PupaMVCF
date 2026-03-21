using Grpc.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Macro;

public abstract class MacroApp : BaseApp, ISecureMacroAppProvider {
   protected readonly Server _server;
   protected bool _serverShutdown = false;
   public static ISecureMacroAppProvider SecureInstance { get; private set; } = null!;

   protected MacroApp(IConfiguration configuration,
      IDatabaseProcessor databaseProcessor, IEnumerable<ServerServiceDefinition> services,
      ILogger<BaseApp> logger) : base(configuration,
      databaseProcessor, logger) {
      if (SecureInstance != null)
         throw new InvalidOperationException("MacroApp provider has already been configured");
      _server = new Server {
         Ports = {
            new ServerPort(HostName, Port, ServerCredentials.Insecure)
         }
      };
      foreach (var serviceItem in services) _server.Services.Add(serviceItem);
      SecureInstance = this;
   }

   public override async Task Run(CancellationToken cancellationToken) {
      Logger.LogInformation("gRPC server run! {HostName}:{Port}", HostName, Port);
      _server.Start();
      try {
         await Task.Delay(System.Threading.Timeout.Infinite, cancellationToken);
      } finally {
         await Stop(cancellationToken);
      }
   }

   public override async Task Stop(CancellationToken cancellationToken) {
      if (_serverShutdown) return;
      try {
         Logger.LogInformation("gRPC server shutdown! {HostName}:{Port}", HostName, Port);
         _serverShutdown = true;
         await _server.ShutdownAsync();
      } finally {
         Dispose();
      }
   }

   #region DISPOSE

   public override void Dispose() {
      SecureInstance = null!;
      base.Dispose();
   }

   #endregion
}