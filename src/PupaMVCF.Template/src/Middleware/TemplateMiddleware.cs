using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Template.Middleware;

public sealed class TemplateMiddleware(ILogger<TemplateMiddleware> logger) : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      logger.LogWarning("Template middleware!");
      return Option.OkTask();
   }
}