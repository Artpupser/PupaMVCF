using Microsoft.Extensions.Logging;

using PupaLib.Core;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Web.Template.Middleware;

public sealed class TemplateMiddleware : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      WebApp.Context.Logger.LogWarning("Template middleware!");
      return Option.OkTask();
   }
}