using Grpc.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Macro;

public abstract class MacroApp : IHostedService, ISecureMacroAppContext {
   protected readonly Server _server;
   public VirtualFolder PublicFolder { get; }
   public IConfiguration Configuration { get; }
   public ILogger<MacroApp> Logger { get; }
   public static ISecureMacroAppContext SecureContextInstance { get; private set; } = null!;
   [ConfigurationKeyName("Host")] public static string HostName { get; private set; } = string.Empty;

   [ConfigurationKeyName("Port")] public static ushort Port { get; private set; } = 50001;

   protected MacroApp(IConfiguration configuration, IEnumerable<ServerServiceDefinition> services,
      ILogger<MacroApp> logger) {
      if (SecureContextInstance != null)
         throw new InvalidOperationException("MacroApp provider has already been configured");
      configuration.BindConfigurationWithClass(this);
      Configuration = configuration;
      PublicFolder = VirtualIo.RootFolder.GetFolderIn("public") ??
                     throw new DirectoryNotFoundException("Public folder not founded");
      Logger = logger;
      _server = new Server {
         Ports = {
            new ServerPort(HostName, Port, ServerCredentials.Insecure)
         }
      };
      foreach (var serviceItem in services) _server.Services.Add(serviceItem);
      SecureContextInstance = this;
   }

   public async Task StartAsync(CancellationToken cancellationToken) {
      Logger.LogInformation("gRPC server run! {HostName}:{Port}", HostName, Port);
      _server.Start();
   }

   public async Task StopAsync(CancellationToken cancellationToken) {
      Logger.LogWarning("gRPC server shutdown! {HostName}:{Port}", HostName, Port);
      await _server.ShutdownAsync();
   }
}