using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

public sealed class NeedValidatorModule : IValidatorModule {
   public string RuleId => "need";
   public string Message => "Value is null";

   public Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      return Task.FromResult(instance != null);
   }
}