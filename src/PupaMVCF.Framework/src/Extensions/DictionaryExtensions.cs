using PupaMVCF.Framework.Core.Extra;

namespace PupaMVCF.Framework.Extensions;

public static class DictionaryExtensions {
   public static BiFrozenDictionary<TKey, TValue> ToBiFrozenDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dict)
      where TValue : notnull
      where TKey : notnull {
      return new BiFrozenDictionary<TKey, TValue>(dict.Keys.ToArray(), dict.Values.ToArray());
   }
}