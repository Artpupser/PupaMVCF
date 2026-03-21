using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Views;


namespace PupaMVCF.Framework;

public abstract class BaseApp : ISecureAnyAppProvider, IDisposable {
   public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web) {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      PropertyNameCaseInsensitive = true,
      WriteIndented = false
   };

   public IDatabaseProcessor DatabaseProcessor { get; }
   public VirtualFolder PublicFolder { get; }
   public IConfiguration Configuration { get; }
   public ILogger<BaseApp> Logger { get; }
   public static ISecureAnyAppProvider BaseInstance { get; private set; } = null!;

   [ConfigurationKeyName("Host")] public static string HostName { get; private set; } = string.Empty;

   [ConfigurationKeyName("Port")] public static ushort Port { get; private set; } = 50001;

   [ConfigurationKeyName("Timeout")] public static TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(10);

   [ConfigurationKeyName("CaptchaSecureKey")]
   public static string CaptchaSecureKey { get; private set; } = string.Empty;

   [ConfigurationKeyName("CaptchaSecureSite")]
   public static string CaptchaSecureSite { get; private set; } = string.Empty;

   protected BaseApp(IConfiguration configuration, IDatabaseProcessor databaseProcessor, ILogger<BaseApp> logger) {
      if (BaseInstance != null) throw new InvalidOperationException("BaseApp provider has already been configured");
      configuration.BindConfigurationWithClass(this);
      View.LoadCssFiles(configuration);
      Configuration = configuration;
      Logger = logger;
      DatabaseProcessor = databaseProcessor;
      PublicFolder = VirtualIo.RootFolder.GetFolderIn("public") ??
                     throw new DirectoryNotFoundException("Public folder not founded");
      BaseInstance = this;
   }

   public abstract Task Run(CancellationToken cancellationToken);
   public abstract Task Stop(CancellationToken cancellationToken);

   public virtual void Dispose() {
      BaseInstance = null!;
   }
}