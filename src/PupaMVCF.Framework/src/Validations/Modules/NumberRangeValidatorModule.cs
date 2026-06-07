using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

[InitializatorEye(included: true)]
public sealed class NumberRangeValidatorModule(IValidatorManager validatorManager) : ValidatorModule(validatorManager) {
   public override string RuleId => "number_range";
   public override string Message => "Number value not in range";

   public override Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not float value)
         return Task.FromResult(false);
      var split = options.Split(' ').AsSpan();
      var min = float.Parse(split[0]);
      var max = float.Parse(split[1]);
      return Task.FromResult(value >= min && value <= max);
   }
}
