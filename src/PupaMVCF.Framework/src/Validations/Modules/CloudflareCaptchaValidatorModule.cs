using System.Text.Json;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Models;

namespace PupaMVCF.Framework.Validations.Modules;

public sealed class CloudflareCaptchaValidatorModule : IValidatorModule {
   private const string CLOUDFLARE_TURNSTILE = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
   public string RuleId => "cloudflare_captcha";
   public string Message => "Captcha not valid";
   public static string Secret => WebApp.Context.Configuration.GetAny<string>("CaptchaSecureKey");

   public async Task<bool> Valid(object? instance, string options, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance is not string value) return false;
      using var content = new FormUrlEncodedContent(new Dictionary<string, string> {
         ["secret"] = Secret,
         ["token"] = value
      });
      using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CLOUDFLARE_TURNSTILE) {
         Content = content
      };
      using var httpResponseMessage = await WebApp.Context.Client
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