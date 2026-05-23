using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Validations;
using PupaMVCF.Tests.Models;

namespace PupaMVCF.Tests.Validations.Modules;

public sealed class TestValidatorModule(IValidatorManager validatorManager) : ValidatorModule(validatorManager) {
   public override async Task<bool> Valid(object? instance, string options, Request request, Response response,
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
            await ValidatorManager.Valid(model, request, response, cancellationToken);
         if (!result)
            return false;
      }

      return true;
   }

   public override string RuleId => "test_items";
   public override string Message => "TestModel not correct";
}