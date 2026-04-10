using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Controllers;

public sealed class ErrorControllerOnlyJson : Controller {
   private ValueTask ErrorJsonHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.WriteTJsonToCache(response.Errors);
      return ValueTask.CompletedTask;
   }

   [ControllerHandler("*", HttpMethodType.GET, typeof(ErrorMiddleware))]
   private async Task ErrorPageHandler(Request request, Response response,
      CancellationToken cancellationToken) {
      await ErrorJsonHandler(request, response, cancellationToken);
   }
}