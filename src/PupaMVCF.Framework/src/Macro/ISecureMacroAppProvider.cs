using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

using PupaMVCF.Framework.Database;

namespace PupaMVCF.Framework.Macro;

public interface ISecureMacroAppProvider {
   public VirtualFolder PublicFolder { get; }
   public IDatabaseProcessor DatabaseProcessor { get; }
   public IConfiguration Configuration { get; }
   public ILogger<MacroApp> Logger { get; }
}