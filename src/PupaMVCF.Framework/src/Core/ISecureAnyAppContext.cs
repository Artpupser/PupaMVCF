using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

namespace PupaMVCF.Framework.Core;

public interface ISecureAnyAppContext<out T> where T : notnull {
   public VirtualFolder PublicFolder { get; }
   public ILogger<T> Logger { get; }
   public IConfiguration Configuration { get; }
}