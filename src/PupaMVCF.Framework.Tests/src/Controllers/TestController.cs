using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Tests.Models;

namespace PupaMVCF.Framework.Tests.Controllers;

public sealed class TestController : Controller {
   [ControllerHandler("/test_post", HttpMethodType.POST, typeof(LoggerMiddleware))]
   private async Task TestPostHandler(Request request, Response response, CancellationToken cancellationToken) {
      var optionValid =
         await WebApp.Context.Validator.ValidFromRequest<TestModel>(request, response, cancellationToken);
      if (optionValid.Out(out var model)) {
         WebApp.Context.Logger.LogInformation(model.Id);
         foreach (var modelItem in model.Items) {
            WebApp.Context.Logger.LogInformation(modelItem.Name);
            WebApp.Context.Logger.LogInformation(modelItem.Age.ToString());
            WebApp.Context.Logger.LogInformation(modelItem.Email);
         }
      }

      response.WriteStrToCache(string.Empty);
   }

   [ControllerHandler("/test_get", HttpMethodType.GET, typeof(LoggerMiddleware))]
   private async Task TestGetHandler(Request request, Response response, CancellationToken cancellationToken) {
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
   }
}