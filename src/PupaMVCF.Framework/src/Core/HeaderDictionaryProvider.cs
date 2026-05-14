using Microsoft.AspNetCore.Http;

using PupaLib.Core;

namespace PupaMVCF.Framework.Core;

public sealed class HeaderDictionaryProvider(IHeaderDictionary headerDictionary) : IRequestHeader {
   public Option<string> UserAgent => Get("User-Agent");
   public Option<string> Authorization => Get("Authorization");
   public Option<string> Accept => Get("Accept");
   public Option<string> Host => Get("Host");
   public Option<string> AcceptLanguage => Get("Accept-Language");

   public Option<string> Get(string key) {
      var result = headerDictionary[key].ToString();
      return string.IsNullOrWhiteSpace(result) ? Option<string>.Fail() : Option<string>.Ok(result);
   }
}