using PupaMVCF.ExampleProcess.Models;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.ExampleProcess.Controllers;

public record FruitResponse(string Message);

public sealed class FruitsController : Controller {
   [ControllerHandler("/api/fruit/add", HttpMethodType.POST)]
   private async Task AddFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      var validOption =
         await WebApp.Context.Validator.ValidFromRequest<FruitModel>(request, response, cancellationToken);
      if (validOption.Out(out var model))
         response.WriteTJsonToCache(new FruitResponse($"Added {model.Name}, {model.Amount}"));
   }

   [ControllerHandler("/api/fruit/remove", HttpMethodType.POST)]
   private async Task RemoveFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      var validOption =
         await WebApp.Context.Validator.ValidFromRequest<FruitModel>(request, response, cancellationToken);
      if (validOption.Out(out var model))
         response.WriteTJsonToCache(new FruitResponse($"Removed {model.Name}"));
   }
}