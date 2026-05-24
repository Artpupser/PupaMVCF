using Microsoft.Extensions.Configuration;

namespace PupaMVCF.Framework.Core;

public interface IAnyAppContext<out T> where T : notnull {
   public IConfiguration Configuration { get; }
}