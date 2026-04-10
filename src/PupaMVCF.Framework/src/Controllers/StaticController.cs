using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

public sealed class StaticController : Controller {
   private static readonly char[] InvalidPathChars = ['\\', '/', '.', '\0', ':', '*', '?', '"', '<', '>', '|'];

   [ControllerHandler("/api/public/files", HttpMethodType.GET)]
   private async Task GetPublicFileHandler(Request request, Response response, CancellationToken cancellationToken) {
      if (!request.GetQueryValue("name").Out(out var name)) {
         response.PushError("Path not valid", 400);
         return;
      }

      cancellationToken.ThrowIfCancellationRequested();

      var file = WebApp.Context.PublicFolder.GetFileIn(name);

      if (file is null) {
         response.PushError("File not found", 404);
         return;
      }

      response.SetCache(TimeSpan.FromDays(1));
      await response.WriteVirtualFileToCache(file, cancellationToken);
   }
}