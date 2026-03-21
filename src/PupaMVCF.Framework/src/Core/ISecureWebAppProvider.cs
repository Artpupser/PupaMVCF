using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Database;

namespace PupaMVCF.Framework.Core;

public interface ISecureWebAppProvider {
   public VirtualFolder PublicFolder { get; }
   public IDatabaseProcessor DatabaseProcessor { get; }
   public IConfiguration Configuration { get; }
   public ILogger<WebApp> Logger { get; }
   public HttpClient Client { get; }
}