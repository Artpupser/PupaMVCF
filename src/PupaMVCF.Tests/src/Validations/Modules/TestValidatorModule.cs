using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Tests.Models;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Tests.Validations.Modules;

public sealed class TestValidatorModule : IValidatorModule {
   public async Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not TestModelItem[] models) {
         response.PushError("Object is not TestModelItem[]");
         return false;
      }

      if (models.Length == 0) {
         response.PushError("Test items = 0");
         return false;
      }

      foreach (var model in models) {
         var result =
            await WebApp.Context.Validator.Valid<TestModelItem>(model, request, response, cancellationToken);
         if (!result)
            return false;
      }

      return true;
   }

   public string RuleId => "test_items";
   public string Message => "TestModel not correct";
}