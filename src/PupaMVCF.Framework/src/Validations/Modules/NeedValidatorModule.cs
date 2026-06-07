using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

[InitializatorEye(included: true)]
public sealed class NeedValidatorModule(IValidatorManager validatorManager) : ValidatorModule(validatorManager) {
   public override string RuleId => "need";
   public override string Message => "Value is null";

   public override Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      return Task.FromResult(instance != null);
   }
}
