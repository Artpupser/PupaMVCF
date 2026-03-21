using System.Collections.Frozen;

namespace PupaMVCF.Framework.Core;

public enum HttpMethodType : byte {
   GET = 0,
   POST = 1,
   PUT = 2,
   HEAD = 3,
   DELETE = 4,
   PATCH = 5,
   OPTIONS = 6,
   CONNECT = 7,
   TRACE = 8
}

public static class HttpMethodManager {
   private static readonly FrozenDictionary<string, HttpMethodType> _httpMethods =
      new Dictionary<string, HttpMethodType> {
         [nameof(HttpMethodType.GET)] = HttpMethodType.GET,
         [nameof(HttpMethodType.POST)] = HttpMethodType.POST,
         [nameof(HttpMethodType.PUT)] = HttpMethodType.PUT,
         [nameof(HttpMethodType.HEAD)] = HttpMethodType.HEAD,
         [nameof(HttpMethodType.DELETE)] = HttpMethodType.DELETE,
         [nameof(HttpMethodType.PATCH)] = HttpMethodType.PATCH,
         [nameof(HttpMethodType.OPTIONS)] = HttpMethodType.OPTIONS,
         [nameof(HttpMethodType.CONNECT)] = HttpMethodType.CONNECT,
         [nameof(HttpMethodType.TRACE)] = HttpMethodType.TRACE
      }.ToFrozenDictionary();

   public static bool TryGetHttpMethod(string value, out HttpMethodType methodType) {
      return _httpMethods.TryGetValue(value, out methodType);
   }

   public static HttpMethodType HardGetHttpMethod(string value) {
      return _httpMethods[value];
   }
}