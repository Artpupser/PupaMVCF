using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Web.Template.Middleware;

public sealed class TemplateMiddleware : IMiddleware {
   public Task<bool> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      WebApp.SecureContextInstance.Logger.LogWarning("Template middleware!");
      return Task.FromResult(true);
   }
}