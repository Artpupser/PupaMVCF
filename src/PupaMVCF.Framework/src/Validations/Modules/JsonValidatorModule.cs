using System.Text.Json;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

[InitializatorEye(included: true)]
public sealed class JsonValidatorModule(IValidatorManager validatorManager) : ValidatorModule(validatorManager) {
   public override string RuleId => "json";
   public override string Message => "This object not json";

   public override Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      try {
         if (instance is not string str) return Task.FromResult(false);
         JsonDocument.Parse(str);
         return Task.FromResult(true);
      } catch {
         return Task.FromResult(false);
      }
   }
}
