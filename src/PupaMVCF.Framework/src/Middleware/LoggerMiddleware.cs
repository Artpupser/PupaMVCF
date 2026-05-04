using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

public sealed class LoggerMiddleware : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      WebApp.Context.Logger.LogInformation(request.ToString());
      return Option.OkTask();
   }
}