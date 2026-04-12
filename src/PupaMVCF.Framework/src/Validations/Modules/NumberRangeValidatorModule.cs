using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

public sealed class NumberRangeValidatorModule : IValidatorModule {
   public string RuleId => "number_range";
   public string Message => "Number value not in range";

   public Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not float value)
         return Task.FromResult(false);
      var split = options.Split(' ').AsSpan();
      var min = float.Parse(split[0]);
      var max = float.Parse(split[1]);
      return Task.FromResult(value >= min && value <= max);
   }
}