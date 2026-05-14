using System.Collections.Frozen;
using System.Reflection;

using PupaLib.Core;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public sealed class RouterMap {
   private readonly FrozenDictionary<RouteKey, RouteValue>
      _routes;

   public RouteValue? Error { get; }

   public RouterMap(RouterMapBuilder builder) {
      var controllerTypes = builder.Build();
      var dict = new Dictionary<RouteKey, RouteValue>();
      foreach (var controllerType in controllerTypes) {
         var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.IsDefined(typeof(ControllerHandlerAttribute), false));

         foreach (var method in methods) {
            var attr = method.GetCustomAttribute<ControllerHandlerAttribute>()!;

            dict.Add(
               new RouteKey(attr.Pattern, attr.HttpMethodType),
               new RouteValue(controllerType, method, attr.Middlewares)
            );
         }
      }

      _routes = dict.ToFrozenDictionary();
      Error = _routes.FirstOrDefault(x => x.Key.Pattern == "*").Value;
   }

   public Option<RouteValue> GetRoute(Request request) {
      try {
         var result = _routes[request.ToRouteKey()];
         return Option<RouteValue>.Ok(result);
      } catch {
         return Option<RouteValue>.Fail();
      }
   }
}