using Microsoft.Extensions.Configuration;

namespace PupaMVCF.Framework.Extensions;

public static class ConfigurationExtensions {
   public static TimeSpan GetTimeSpan(this IConfiguration configuration, string key) {
      return TimeSpan.Parse(configuration[key] ?? throw new NullReferenceException());
   }

   public static T GetAny<T>(this IConfiguration configuration, string key) {
      return (T)Convert.ChangeType(configuration[key] ?? throw new NullReferenceException(), typeof(T));
   }
}