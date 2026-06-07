using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

[InitializatorEye(true)]
public sealed class ErrorMiddleware(ILogger<ErrorMiddleware> logger) : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      var i = 0;
      foreach (var error in response.Errors) {
         logger.LogInformation("[{I}] {Error}", i, error);
         i++;
      }

      return Option.OkTask();
   }
}
