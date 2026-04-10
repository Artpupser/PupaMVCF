using System.Collections.Frozen;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Routing;

public sealed class RouterMap {
   private readonly FrozenDictionary<RouteKey, RouteValue>
      _routes;

   private readonly FrozenDictionary<Type, IMiddleware> _middlewares;
   public RouteValue? Error { get; }

   public RouterMap(RouterMapBuilder builder) {
      _routes = builder.BuildRoutes();
      _middlewares = builder.BuildMiddlewares();
      Error = _routes.FirstOrDefault(x => x.Key.Pattern == "*").Value;
   }

   public IMiddleware GetMiddleware(Type type) {
      return _middlewares[type];
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