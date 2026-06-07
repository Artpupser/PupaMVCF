using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

[InitializatorEye(true)]
public sealed class LoggerMiddleware(ILogger<LoggerMiddleware> logger) : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      logger.LogInformation(request.ToString());
      return Option.OkTask();
   }
}
