using PupaMVCF.ExampleProcess.Views;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Components;

namespace PupaMVCF.ExampleProcess.Controllers;

public sealed class PagesController(ILogger<PagesController> logger) : Controller {
   [ControllerHandler("/", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private async Task MainPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new MainPageView();
      logger.Log(LogLevel.Warning, request.Session.Id);
      await SendPage(request, response, view, cancellationToken);
   }

   private static async Task SendPage(Request request, Response response, View view,
      CancellationToken cancellationToken) {
      await view.Html(request, response, cancellationToken);
      response.MimeContentType = MimeContentType.Html;
      response.WriteViewToCache(view);
   }
}