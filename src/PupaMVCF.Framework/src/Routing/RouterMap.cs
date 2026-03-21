using System.Collections.Frozen;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Routing;

public sealed class RouterMap {
   private readonly FrozenDictionary<(string pattern, HttpMethodType methodType), RouteValue>
      _routes;

   private readonly FrozenDictionary<Type, IMiddleware> _middlewares;
   public RouteValue? Error { get; }

   public RouterMap(RouterMapBuilder builder) {
      _routes = builder.BuildRoutes();
      _middlewares = builder.BuildMiddlewares();
      Error = _routes.FirstOrDefault(x => x.Key.pattern == "*").Value;
   }

   public IMiddleware GetMiddleware(Type type) {
      return _middlewares[type];
   }

   public RouteValue GetRoute(Request request) {
      return _routes[(request.RawUrl, request.HttpMethodType)];
   }

   public bool TryGetRoute(Request request, out RouteValue routeValue) {
      return _routes.TryGetValue((request.RawUrl, request.HttpMethodType), out routeValue);
   }
}