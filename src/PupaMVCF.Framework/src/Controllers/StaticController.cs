using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

[InitializatorEye(true)]
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

      if (!publicFolder.Virtual.GetFileIn(name).Out(out var file)) {
         response.PushError("File not found", 404);
         return;
      }

      response.SetCache(TimeSpan.FromDays(1));
      await response.WriteVirtualFileToCache(file, cancellationToken);
   }
}
