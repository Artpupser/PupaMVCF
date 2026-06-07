using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Controllers;

public static class InitializatorBuilder {
   private static readonly HashSet<Type> _typesSet;

   static InitializatorBuilder() {
      var hashSet = new HashSet<Type>();
      foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach(var type in assembly.GetTypes().Where(x => x.IsDefined(typeof(InitializatorEyeAttribute), false) && x.GetCustomAttribute<InitializatorEyeAttribute>(false)!.Included)) {
           hashSet.Add(type);
        }
      }
      _typesSet = hashSet;
   }

   public static ValueTask Except(params Type[] types) {
      foreach(var type in types) {
         _typesSet.Remove(type);
      }
      return ValueTask.CompletedTask;
   }

   public static IEnumerable<Type> Build<T>() {
      return _typesSet.Where(x => x.IsAssignableTo(typeof(T)));
   }

   public static IEnumerable<T> CreateInstances<T>(IServiceProvider serviceProvider, params object[] parameters) {
      return Build<T>().Select(x=> (T)ActivatorUtilities.CreateInstance(serviceProvider,x,parameters));
   }

   public static ValueTask PreloadMvcComponents(IServiceCollection serviceCollection) {
      foreach(var type in Build<IMiddleware>()) {
         serviceCollection.AddScoped(type);
      }

      foreach(var type in Build<Controller>()) {
         serviceCollection.AddSingleton(type);
      }

      return ValueTask.CompletedTask;
   }

} 
