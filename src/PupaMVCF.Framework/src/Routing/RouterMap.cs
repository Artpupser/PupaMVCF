using System.Collections.Frozen;
using System.Reflection;
using System.Text;

using PupaLib.Core;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public sealed class RouterMap {
   private readonly FrozenDictionary<RouteKey, RouteValue> _routes;

   public override string ToString() {
      var sb = new StringBuilder();
      foreach (var route in _routes) sb.Append($"[{route.Key.Method}]\t<{route.Key.Pattern}>\n");
      return sb.ToString();
   }

   public RouteValue? Error { get; }

   public RouterMap() {
      var controllerTypes = InitializatorBuilder.Build<Controller>();
      var dict = new Dictionary<RouteKey, RouteValue>();
      foreach (var controllerType in controllerTypes) {
         var scheme = controllerType.GetCustomAttribute<ControllerSchemeAttribute>();
         var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.IsDefined(typeof(ControllerHandlerAttribute), false));

         foreach (var method in methods) {
            var attr = method.GetCustomAttribute<ControllerHandlerAttribute>()!;
            var pattern = string.Empty;
            if (scheme != null) pattern += scheme.PatternPrefix;
            pattern += attr.Pattern;
            dict.Add(
               new RouteKey(pattern, attr.HttpMethodType),
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
