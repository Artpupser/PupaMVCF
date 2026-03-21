using System.Text.Json.Serialization;

using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Models;

public record CaptchaModel() {
   [ValidRule(ValidRuleType.Need)]
   [ValidRule(ValidRuleType.CloudflareCaptcha)]
   [JsonPropertyName("cf-turnstile-response")]
   public string Captcha { get; set; } = string.Empty;
}