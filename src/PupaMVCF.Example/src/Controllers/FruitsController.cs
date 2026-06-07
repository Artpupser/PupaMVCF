using PupaMVCF.Example.Models;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Example.Controllers;

public record FruitResponse(string Message);

[InitializatorEye(true)]
[ControllerScheme("/fruit")]
public sealed class FruitsController(IValidatorManager validator) : Controller {
   private readonly List<FruitModel> _storage = [];
#region GET
   [ControllerHandler("/list", HttpMethodType.GET)]
   private async Task ListFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.WriteTJsonToCache(_storage);
   }
#endregion

#region POST
   [ControllerHandler("/add", HttpMethodType.POST)]
   private async Task AddFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      if (!(await validator.ValidFromRequest<FruitModel>(request, response, cancellationToken)).Out(out var model))
         return;
      _storage.Add(model);
      response.WriteTJsonToCache(new FruitResponse($"Added {model.Name}, {model.Amount}"));
   }

   [ControllerHandler("/remove", HttpMethodType.POST)]
   private async Task RemoveFruitHandler(Request request, Response response, CancellationToken cancellationToken) {
      if (!(await validator.ValidFromRequest<FruitModel>(request, response, cancellationToken)).Out(out var model))
         return;
      var target = _storage.FirstOrDefault(x=>x.Name==model.Name);
      if(target is not null) {
         _storage.Remove(target);
      }
      response.WriteTJsonToCache(new FruitResponse($"Removed {model.Name}"));
   }
#endregion
}
