using System.Collections.Frozen;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;

namespace PupaMVCF.Framework.Controllers;

public abstract class BasicErrorController : Controller {
   private readonly FrozenDictionary<MimeContentType, Func<Request, Response, CancellationToken, Task>> _errorHandlers;

   protected BasicErrorController() {
      _errorHandlers = new Dictionary<MimeContentType, Func<Request, Response, CancellationToken, Task>> {
         [MimeContentType.Json] = ErrorJsonHandler,
         [MimeContentType.Js] = ErrorJsonHandler,
         [MimeContentType.Text] = ErrorJsonHandler,
         [MimeContentType.Xml] = ErrorXmlHandler,
         [MimeContentType.Undefined] = ErrorHtmlHandler,
         [MimeContentType.Html] = ErrorHtmlHandler
      }.ToFrozenDictionary();
   }

   protected virtual Task ErrorXmlHandler(Request request, Response response, CancellationToken cancellationToken) {
      throw new NotImplementedException();
   }

   protected virtual Task ErrorHtmlHandler(Request request, Response response, CancellationToken cancellationToken) {
      throw new NotImplementedException();
   }

   protected virtual Task ErrorJsonHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.WriteTJsonToCache(response.StackContentList);
      return Task.CompletedTask;
   }

   [ControllerHandler("*", HttpMethodType.GET)]
   protected virtual async Task ErrorPageHandler(Request request, Response response,
      CancellationToken cancellationToken) {
      if (_errorHandlers.TryGetValue(request.MimeContentType, out var func))
         await func(request, response, cancellationToken);
   }
}