using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations.Modules;

public partial class EmailValidatorModule : IValidatorModule {
   public string RuleId => "email";
   public string Message => "Value is not email";

   public Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      return Task.FromResult(instance is string value && EmailRegex().IsMatch(value));
   }

   [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
      System.Text.RegularExpressions.RegexOptions.Singleline)]
   private static partial System.Text.RegularExpressions.Regex EmailRegex();
}