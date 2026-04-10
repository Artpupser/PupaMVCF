using Microsoft.Extensions.Logging;

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


   private async Task InvokeErrorRoute(Request request, Response response, CancellationToken cancellationToken) {
      if (_map.Error != null) {
         await _map.Error?.Handler(request, response, cancellationToken)!;
         return;
      }

      response.StatusCode = 400;
      response.WriteStrToCache($"Error handler not found, status {response.StatusCode}");
   }

   private async Task<Option> InvokeRoute(RouteValue? routeValue, Request request, Response response,
      CancellationToken cancellationToken) {
      if (routeValue == null) return Option.Fail();
      var route = routeValue.Value;
      if (route.Middlewares.Count > 0) {
         var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         var queue = route.GetQueueMiddlewareTypes();
         while (queue.Count > 0) {
            var item = queue.Dequeue();
            var middleware = _map.GetMiddleware(item);
            try {
               var result = await middleware.Invoke(request, response, cts.Token);
               if (result) continue;
               response.PushError($"Middleware {item.Name} returned false, stopping execution of route");
               return Option.Fail();
            } catch (OperationCanceledException) {
               response.PushError($"Middleware {item.Name} cancelled");
               return Option.Fail();
            }
         }
      }

      await route.Handler(request, response, cancellationToken);
      return Option.Ok();
   }

   public async Task Execute(Request request, Response response, CancellationToken cancellationToken) {
      if (_map.GetRoute(request).Out(out var routeValue)) {
         var optionRoute = await InvokeRoute(routeValue, request, response, cancellationToken);
         if (!optionRoute || response.Errors.Any())
            await InvokeErrorRoute(request, response, cancellationToken);
         return;
      }

      response.PushError("Route not found", 404);
      await InvokeErrorRoute(request, response, cancellationToken);
   }
}