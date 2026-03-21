using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public sealed class Router : IRouter {
   private readonly RouterMap _map;

   public Router(RouterMap map) {
      _map = map;
   }

   public Router(RouterMapBuilder mapBuilder) {
      _map = new RouterMap(mapBuilder);
   }

   private async Task InvokeRoute(Request request, Response response, RouteValue? routeValue,
      CancellationToken cancellationToken) {
      if (routeValue == null) return;
      var route = routeValue.Value;
      if (route.Middlewares.Count > 0) {
         var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         var queue = route.GetQueueMiddlewares();
         var item = queue.Dequeue();
         var middleware = _map.GetMiddleware(item);
         try {
            while (queue.Count > 0) {
               if (await middleware.Invoke(request, response, cts.Token)) continue;
               response.ErrorStack.PushStack($"Middleware {item.Name} returned false, stopping execution of route");
               return;
            }
         } catch (OperationCanceledException) {
            response.ErrorStack.PushStack($"Middleware {item.Name} cancelled");
            return;
         }
      }

      await route.Handler(request, response, cancellationToken);
   }

   public async Task Execute(Request request, Response response, CancellationToken cancellationToken) {
      if (!Session.VerifySession(request)) Session.GenerateSessionGUIDCookie(response);

      if (_map.TryGetRoute(request, out var routeValue)) {
         await InvokeRoute(request, response, routeValue, cancellationToken);
         if (response.ErrorStack.LengthStack > 0 || response.StatusCode != 200)
            await InvokeRoute(request, response, _map.Error, cancellationToken);
         return;
      }

      response.StatusCode = 404;
      await InvokeRoute(request, response, _map.Error, cancellationToken);
   }
}