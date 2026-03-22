using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

public sealed class LoggerMiddleware : IMiddleware {
   public Task<bool> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      WebApp.SecureContextInstance.Logger.LogInformation(
         "<- Request ->\nPattern:{Pattern}\nMimeType:{MimeType}\nMethod:{Method}\n", request.RawUrl,
         request.MimeContentType, request.HttpMethodType);
      return Task.FromResult(true);
   }
}