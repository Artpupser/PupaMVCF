using PupaMVCF.ExampleProcess.Views;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Components;

namespace PupaMVCF.ExampleProcess.Controllers;

public sealed class PagesController : Controller {
   [ControllerHandler("/", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private async Task MainPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new MainPageView();
      await SendPage(request, response, view, cancellationToken);
   }

   [ControllerHandler("/auth", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private async Task AuthPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new AuthPageView();
      await SendPage(request, response, view, cancellationToken);
   }

   [ControllerHandler("/verify", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private async Task VerifyPageHandler(Request request, Response response, CancellationToken cancellationToken) {
      var view = new VerifyPageView();
      await SendPage(request, response, view, cancellationToken);
   }

   private static async Task SendPage(Request request, Response response, View view,
      CancellationToken cancellationToken) {
      await view.Html(request, response, cancellationToken);
      response.MimeContentType = MimeContentType.Html;
      response.WriteViewToCache(view);
   }
}