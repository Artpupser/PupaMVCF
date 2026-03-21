using Grpc.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Macro;

public abstract class MacroApp : IHostedService, ISecureMacroAppProvider {
   protected readonly Server _server;
   protected bool _serverShutdown = false;
   public VirtualFolder PublicFolder { get; }
   public IDatabaseProcessor DatabaseProcessor { get; }
   public IConfiguration Configuration { get; }
   public ILogger<MacroApp> Logger { get; }
   public static ISecureMacroAppProvider SecureInstance { get; private set; } = null!;
   [ConfigurationKeyName("Host")] public static string HostName { get; private set; } = string.Empty;

   [ConfigurationKeyName("Port")] public static ushort Port { get; private set; } = 50001;

   protected MacroApp(IConfiguration configuration,
      IDatabaseProcessor databaseProcessor, IEnumerable<ServerServiceDefinition> services,
      ILogger<MacroApp> logger) {
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

   public async Task StartAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("gRPC server run! {HostName}:{Port}", HostName, Port);
      _server.Start();
      try {
         await Task.Delay(Timeout.Infinite, cancellationToken);
      } finally {
         await StopAsync(cancellationToken);
      }
   }

   public async Task StopAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("gRPC server shutdown! {HostName}:{Port}", HostName, Port);
      await _server.ShutdownAsync();
   }
}