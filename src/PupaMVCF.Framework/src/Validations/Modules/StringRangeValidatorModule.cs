using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

public sealed class StringRangeValidatorModule : IValidatorModule {
   public string RuleId => "string_range";
   public string Message => "Length string not in range";

   public Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not string value) return Task.FromResult(false);
      var split = options.Split(' ').AsSpan();
      var min = uint.Parse(split[0]);
      var max = uint.Parse(split[1]);
      return Task.FromResult(value.Length >= min && value.Length <= max);
   }
}