using System.Reflection;
using System.Text.Json;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Models;

namespace PupaMVCF.Framework.Validations;

public static partial class ValidatorManager {
   private const string CLOUDFLARE_TURNSTILE = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

   private static string GetMessage(ValidRuleType rule, int value) {
      return rule switch {
         ValidRuleType.Need => "This field is required!",
         ValidRuleType.Number => "The field is not a number",
         ValidRuleType.Min => $"The field must be greater than {value}",
         ValidRuleType.Max => $"The field must be less than {value}",
         ValidRuleType.MinLength => $"The number of characters must be greater than {value}",
         ValidRuleType.MaxLength => $"The number of characters must be less than {value}",
         ValidRuleType.CloudflareCaptcha => $"Captcha verification failed",
         ValidRuleType.Email => $"Invalid email address",
         ValidRuleType.Phone => $"Invalid phone number",
         _ => "Unknown field error"
      };
   }

   private static async Task<bool> Check(ValidRuleType rule, int value, object? content,
      CancellationToken cancellationToken) {
      return content != null && rule switch {
         ValidRuleType.Need => true,
         ValidRuleType.Number => await VerifyNumber($"{content}"),
         ValidRuleType.Min => content is float x && x >= value,
         ValidRuleType.Max => content is float x && x <= value,
         ValidRuleType.MinLength => $"{content}".Length >= value,
         ValidRuleType.MaxLength => $"{content}".Length <= value,
         ValidRuleType.CloudflareCaptcha => await VerifyCaptcha($"{content}", cancellationToken),
         ValidRuleType.Email => await VerifyEmail($"{content}"),
         ValidRuleType.Phone => await VerifyPhone($"{content}"),
         _ => false
      };
   }

   public static async Task<bool> Valid<T>(Request request, Response response, T? instance,
      CancellationToken cancellationToken) {
      if (instance != null) {
         var props = instance.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.IsDefined(typeof(ValidRuleAttribute), false));
         foreach (var prop in props)
         foreach (var attr in prop.GetCustomAttributes<ValidRuleAttribute>()) {
            var check = await Check(attr.Type, attr.Value, prop.GetValue(instance), cancellationToken);
            if (check)
               continue;
            response.ErrorStack.PushStack(GetMessage(attr.Type, attr.Value));
            return false;
         }

         return true;
      }

      response.ErrorStack.PushStack("Data is empty");
      return false;
   }

   private static ValueTask<bool> VerifyNumber(string value) {
      return ValueTask.FromResult(float.TryParse(value, out _));
   }

   private static ValueTask<bool> VerifyEmail(string value) {
      return ValueTask.FromResult(EmailRegex().IsMatch(value));
   }

   private static ValueTask<bool> VerifyPhone(string value) {
      return ValueTask.FromResult(PhoneRegex().IsMatch(PhoneTrashRegex().Replace(value, "")));
   }

   private static async Task<bool> VerifyCaptcha(string value, CancellationToken cancellationToken) {
      using var content = new FormUrlEncodedContent(new Dictionary<string, string> {
         ["secret"] = BaseApp.CaptchaSecureKey,
         ["token"] = value
      });
      using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CLOUDFLARE_TURNSTILE) {
         Content = content
      };
      using var httpResponseMessage = await App.SecureInstance.Client
         .SendAsync(httpRequestMessage, cancellationToken)
         .ConfigureAwait(false);
      if (!httpResponseMessage.IsSuccessStatusCode) return false;
      await using var stream =
         await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
      var result = await JsonSerializer
         .DeserializeAsync(stream, RecaptchaJsonContext.Default.CaptchaResponseModel, cancellationToken)
         .ConfigureAwait(false);
      return result?.Success ?? false;
   }

   [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
   private static partial System.Text.RegularExpressions.Regex EmailRegex();

   [System.Text.RegularExpressions.GeneratedRegex(@"^\+[1-9]\d{1,14}$")]
   private static partial System.Text.RegularExpressions.Regex PhoneRegex();

   [System.Text.RegularExpressions.GeneratedRegex(@"[^\d+]")]
   private static partial System.Text.RegularExpressions.Regex PhoneTrashRegex();
}