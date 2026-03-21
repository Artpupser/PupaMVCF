using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

public sealed class StaticController : Controller {
   [ControllerHandler("/api/public/files", HttpMethodType.GET)]
   private async Task GetPublicFileHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.StatusCode = 404;
      var path = request.GetQueryValue("name");
      if (path == string.Empty || path.Any(x => x is '\\' or '/'))
         return;
      var file = App.SecureInstance.PublicFolder.GetFileIn(path);
      if (file == null)
         return;
      response.StatusCode = 200;
      response.SetCache(TimeSpan.FromDays(1));
      await response.WriteVirtualFileToCache(file, cancellationToken);
   }
}