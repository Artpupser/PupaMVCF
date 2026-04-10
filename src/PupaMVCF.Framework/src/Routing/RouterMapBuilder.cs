using System.Collections.Frozen;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Routing;

public sealed class RouterMapBuilder {
   private readonly Dictionary<RouteKey, RouteValue>
      _routes = [];

   private readonly Dictionary<Type, IMiddleware> _middlewares = [];

   public RouterMapBuilder AddMiddleware(IMiddleware middleware) {
      _middlewares.Add(middleware.GetType(), middleware);
      return this;
   }

   public RouterMapBuilder AddController(Controller controller) {
      foreach (var tuple in controller.GetReflectionHandler())
         _routes.Add(new RouteKey(tuple.Attribute.Pattern, tuple.Attribute.HttpMethodType),
            new RouteValue(tuple.Func, tuple.Attribute.Middlewares));
      return this;
   }

   public RouterMapBuilder AddMiddlewareRange(IEnumerable<IMiddleware> middlewares) {
      foreach (var middleware in middlewares)
         AddMiddleware(middleware);
      return this;
   }

   public RouterMapBuilder AddControllerRange(IEnumerable<Controller> controllers) {
      foreach (var controller in controllers)
         AddController(controller);
      return this;
   }


   public FrozenDictionary<RouteKey, RouteValue>
      BuildRoutes() {
      return _routes.ToFrozenDictionary();
   }

   public FrozenDictionary<Type, IMiddleware> BuildMiddlewares() {
      return _middlewares.ToFrozenDictionary();
   }
}