using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations;

public interface IValidatorModule {
   public string RuleId { get; }
   public string Message { get; }

   public Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken);
}