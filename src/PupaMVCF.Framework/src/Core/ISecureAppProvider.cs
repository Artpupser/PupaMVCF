using PupaLib.FileIO;

using PupaMVCF.Framework.Database;

namespace PupaMVCF.Framework.Core;

public interface ISecureAppProvider {
   public HttpClient Client { get; }
   public VirtualFolder PublicFolder { get; }
   public IDatabaseProcessor DatabaseProcessor { get; }
}