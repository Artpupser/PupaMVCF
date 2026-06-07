using System.Globalization;

using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Tests.Models;

namespace PupaMVCF.Tests.Controllers;

[InitializatorEye(true)]
[ControllerScheme("/test")]
public sealed class TestController(IValidatorManager validatorManager, ILogger<TestController> logger) : Controller {
   [ControllerHandler("/post", HttpMethodType.POST, typeof(LoggerMiddleware))]
   private async Task TestPostHandler(Request request, Response response, CancellationToken cancellationToken) {
      var optionValid =
         await validatorManager.ValidFromRequest<TestModel>(request, response, cancellationToken);
      if (optionValid.Out(out var model)) {
         logger.LogInformation(model.Id);
         foreach (var modelItem in model.Items) {
            logger.LogInformation(modelItem.Name);
            logger.LogInformation(modelItem.Age.ToString(CultureInfo.CurrentCulture));
            logger.LogInformation(modelItem.Email);
         }
      }

      response.WriteStrToCache(string.Empty);
   }

   [ControllerHandler("/get", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private Task TestGetHandler(Request request, Response response, CancellationToken cancellationToken) {
      var testModel = new TestModel {
         Id = "Hello man",
         Items = [
            new TestModelItem {
               Name = "Get",
               Age = 20,
               Email = "get@get.get"
            }
         ]
      };
      response.WriteTJsonToCache(testModel);
      return Task.CompletedTask;
   }
}
