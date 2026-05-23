using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaLib.FileIO;

namespace PupaMVCF.Framework.Core;

public interface IAnyAppContext<out T> where T : notnull {
   public IConfiguration Configuration { get; }
}