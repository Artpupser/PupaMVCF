using PupaMVCF.Framework.Models;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess.Models;

public record RegistrationModel(CaptchaModel Captcha) {
   [ValidRule(ValidRuleType.Need)]
   [ValidRule(ValidRuleType.MaxLength, 64)]
   [ValidRule(ValidRuleType.MinLength, 3)]
   public string Login { get; set; }

   [ValidRule(ValidRuleType.Need)]
   [ValidRule(ValidRuleType.MaxLength, 254)]
   [ValidRule(ValidRuleType.Email)]
   public string Email { get; set; }
}