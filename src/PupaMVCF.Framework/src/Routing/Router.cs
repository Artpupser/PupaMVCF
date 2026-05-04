using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Routing;

public sealed class Router(RouterMapBuilder mapBuilder, IServiceProvider serviceProvider)
   : IRouter {
   private readonly RouterMap _map = new(mapBuilder);


   private async Task InvokeErrorRoute(Request request, Response response, CancellationToken cancellationToken) {
      if (_map.Error != null) {
         await using var serviceScope = serviceProvider.CreateAsyncScope();
         var controller = serviceScope.ServiceProvider.GetRequiredService(_map.Error.Value.ControllerType);
         await (Task)_map.Error.Value.Method.Invoke(controller, [request, response, cancellationToken])!;
         return;
      }

      response.StatusCode = 400;
      response.WriteStrToCache($"Error handler not found, status {response.StatusCode}");
   }

   private async Task<Option> InvokeRoute(RouteValue? routeValue, Request request,
      Response response,
      CancellationToken cancellationToken) {
      if (routeValue == null) return Option.Fail();
      var route = routeValue.Value;
      await using var serviceScope = serviceProvider.CreateAsyncScope();
      var controller = serviceScope.ServiceProvider.GetRequiredService(routeValue.Value.ControllerType);
      if (route.Middlewares.Count > 0) {
         var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         var queue = route.ToQueueMiddlewareTypes();
         while (queue.Count > 0) {
            var item = queue.Dequeue();
            var middleware = (IMiddleware)serviceScope.ServiceProvider.GetRequiredService(item);
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

      await (Task)route.Method.Invoke(controller, [request, response, cancellationToken])!;
      return Option.Ok();
   }

   public async Task Execute(Request request, Response response, CancellationToken cancellationToken) {
      try {
         if (_map.GetRoute(request).Out(out var routeValue)) {
            var optionRoute = await InvokeRoute(routeValue, request, response, cancellationToken);
            if (!optionRoute || response.Errors.Any())
               await InvokeErrorRoute(request, response, cancellationToken);
            return;
         }

         response.PushError("Route not found", 404);
         await InvokeErrorRoute(request, response, cancellationToken);
      } catch (Exception e) {
         WebApp.Context.Logger.LogError("Error {e}", e);
         throw;
      }
   }
}