using System.Text.Json.Serialization;

using PupaMVCF.Framework.Models;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess.Models;

public record LoginModel(CaptchaModel Captcha) {
   [ValidRule(ValidRuleType.Need)]
   [ValidRule(ValidRuleType.MaxLength, 254)]
   [ValidRule(ValidRuleType.Email)]
   [JsonPropertyName("email")]
   public string Email { get; set; }
}