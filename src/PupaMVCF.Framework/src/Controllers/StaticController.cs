using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

[ControllerScheme("/public")]
public sealed class StaticController(PublicFolder publicFolder) : Controller {
   private static readonly char[] InvalidChars = ['\\', '/', '\0', ':', '*', '?', '"', '<', '>', '|'];

   [ControllerHandler("/files", HttpMethodType.GET)]
   private async Task GetPublicFileHandler(Request request, Response response, CancellationToken cancellationToken) {
      if (!request.GetQuery("name").Out(out var name)) {
         response.PushError("Path not valid", 400);
         return;
      }

      if (InvalidChars.Any(symbol => name.Any(x => x == symbol))) {
         response.PushError("Invalid symbol in path", 400);
         return;
      }

      cancellationToken.ThrowIfCancellationRequested();

      var file = publicFolder.Virtual.GetFileIn(name);

      if (file is null) {
         response.PushError("File not found", 404);
         return;
      }

      response.SetCache(TimeSpan.FromDays(1));
      await response.WriteVirtualFileToCache(file, cancellationToken);
   }
}