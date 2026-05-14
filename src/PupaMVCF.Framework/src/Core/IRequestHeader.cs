using PupaLib.Core;

namespace PupaMVCF.Framework.Core;

public interface IRequestHeader {
   public Option<string> UserAgent { get; }
   public Option<string> Authorization { get; }
   public Option<string> Accept { get; }
   public Option<string> Host { get; }
   public Option<string> AcceptLanguage { get; }
   public Option<string> Get(string key);
}