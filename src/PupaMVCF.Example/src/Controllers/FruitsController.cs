using PupaMVCF.Example.Models;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Example.Controllers;

public record FruitResponse(string Message);

[ControllerScheme("/fruit")]
public sealed class FruitsController(IValidatorManager validator) : Controller {
   [ControllerHandler("/add", HttpMethodType.POST)]
   private async Task AddFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      var validOption =
         await validator.ValidFromRequest<FruitModel>(request, response, cancellationToken);
      if (validOption.Out(out var model))
         response.WriteTJsonToCache(new FruitResponse($"Added {model.Name}, {model.Amount}"));
   }

   [ControllerHandler("/remove", HttpMethodType.POST)]
   private async Task RemoveFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      var validOption =
         await validator.ValidFromRequest<FruitModel>(request, response, cancellationToken);
      if (validOption.Out(out var model))
         response.WriteTJsonToCache(new FruitResponse($"Removed {model.Name}"));
   }
}