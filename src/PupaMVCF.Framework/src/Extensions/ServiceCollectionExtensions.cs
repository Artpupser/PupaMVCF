using Microsoft.Extensions.DependencyInjection;

namespace PupaMVCF.Framework.Extensions;

public static class ServiceCollectionExtensions {
   public static void AddScoped(this IServiceCollection serviceCollection, Type[] types) {
      foreach (var type in types) serviceCollection.AddScoped(type);
   }

   public static void AddScoped(this IServiceCollection serviceCollection, (Type, Type)[] types) {
      foreach (var type in types) serviceCollection.AddScoped(type.Item1, type.Item2);
   }

   public static void AddSingleton(this IServiceCollection serviceCollection, Type[] types) {
      foreach (var type in types) serviceCollection.AddSingleton(type);
   }

   public static void AddSingleton(this IServiceCollection serviceCollection, (Type, Type)[] types) {
      foreach (var type in types) serviceCollection.AddSingleton(type.Item1, type.Item2);
   }
}