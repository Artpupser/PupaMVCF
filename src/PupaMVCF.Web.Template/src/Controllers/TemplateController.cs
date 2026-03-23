using PupaMVCF.Web.Template.Views;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Views;
using PupaMVCF.Web.Template.Middleware;

namespace PupaMVCF.Web.Template.Controllers;

public sealed class TemplateController : Controller {
   [ControllerHandler("/", HttpMethodType.GET, typeof(LoggerMiddleware), typeof(TemplateMiddleware))]
   private async Task MainPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new TemplateView();
      await SendPage(request, response, view, cancellationToken);
   }

   private static async Task SendPage(Request request, Response response, View view,
      CancellationToken cancellationToken) {
      response.MimeContentType = MimeContentType.Html;
      await view.Html(request, response, cancellationToken);
      response.WriteViewToCache(view);
   }
}