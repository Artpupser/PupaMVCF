using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Models;

namespace PupaMVCF.Framework.Validations.Modules;

[InitializatorEye(included: true)]
public sealed class CloudflareCaptchaValidatorModule(IConfiguration configuration, IValidatorManager validatorManager)
   : ValidatorModule(validatorManager) {
   private const string CLOUDFLARE_TURNSTILE = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
   public override string RuleId => "cloudflare_captcha";
   public override string Message => "Captcha not valid";

   public override async Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not string value) return false;
      using var content = new FormUrlEncodedContent(new Dictionary<string, string> {
         ["secret"] = configuration.GetValue<string>("CaptchaSecureKey") ?? string.Empty,
         ["token"] = value
      });
      using var http = new HttpClient() {
         Timeout = TimeSpan.FromSeconds(10),
      };
      using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CLOUDFLARE_TURNSTILE) {
         Content = content
      };
      using var httpResponseMessage = await http
         .SendAsync(httpRequestMessage, cancellationToken)
         .ConfigureAwait(false); 
      if (!httpResponseMessage.IsSuccessStatusCode) return false;
      await using var stream =
         await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
      var result = await JsonSerializer
         .DeserializeAsync(stream, CaptchaJsonContext.Default.CaptchaResponseModel, cancellationToken)
         .ConfigureAwait(false);
      return result?.Success ?? false;
   }
}
