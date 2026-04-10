using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace PupaMVCF.Framework.Extensions;

public static class ConfigurationExtensions {
   public static void BindConfigurationWithClass<T>(this IConfiguration configuration, T obj) {
      var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                          BindingFlags.GetProperty).Where(x =>
         x.IsDefined(typeof(ConfigurationKeyNameAttribute), false));
      foreach (var prop in props) {
         var type = prop.PropertyType;
         var name = prop.GetCustomAttribute<ConfigurationKeyNameAttribute>()!.Name;
         var configValue = configuration[name] ??
                           throw new NullReferenceException($"Config with key {name} not found!");
         object? value = null;
         if (type == typeof(TimeSpan)) value = TimeSpan.Parse(configValue);
         else
            value = Convert.ChangeType(configValue, type);
         prop.SetValue(obj, value);
      }
   }

   public static TimeSpan GetTimeSpan(this IConfiguration configuration, string key) {
      return TimeSpan.Parse(configuration[key] ?? throw new NullReferenceException());
   }

   public static T GetAny<T>(this IConfiguration configuration, string key) {
      return (T)Convert.ChangeType(configuration[key] ?? throw new NullReferenceException(), typeof(T));
   }
}