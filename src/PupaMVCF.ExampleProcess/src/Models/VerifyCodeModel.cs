using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess.Models;

public record VerifyCodeModel {
   [ValidRule(ValidRuleType.Need)]
   [ValidRule(ValidRuleType.MinLength, 6)]
   [ValidRule(ValidRuleType.MaxLength, 6)]
   public string Code { get; init; }
}