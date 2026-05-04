using PupaMVCF.Framework.Components;
using PupaMVCF.Web.Template.Views;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Web.Template.Middleware;

namespace PupaMVCF.Web.Template.Controllers;

public sealed class TemplateController : Controller {
   [ControllerHandler("/", HttpMethodType.GET, typeof(LoggerMiddleware), typeof(TemplateMiddleware))]
   private async Task MainPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new TemplateView();
      await SendPage(view, request, response, cancellationToken);
   }

   private static async Task SendPage(View view, Request request, Response response,
      CancellationToken cancellationToken) {
      response.MimeContentType = MimeContentType.Html;
      await view.Html(request, response, cancellationToken);
      response.WriteViewToCache(view);
   }
}