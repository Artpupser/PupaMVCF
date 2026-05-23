using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations;

public abstract class ValidatorModule(IValidatorManager validatorManager) {
   protected readonly IValidatorManager ValidatorManager = validatorManager;
   public abstract string RuleId { get; }
   public abstract string Message { get; }

   public abstract Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken);
}