using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations;

public sealed class ModifyValidatorManager : IValidatorManager {
   public readonly IReadOnlyDictionary<string, IValidatorModule> _modules;
   private readonly ConcurrentDictionary<Type, PropertyValidInfo[]> _cachedProperties;
   private IConfiguration Configuration { get; }

   public ModifyValidatorManager(IConfiguration configuration, IEnumerable<IValidatorModule> modules) {
      _cachedProperties = new ConcurrentDictionary<Type, PropertyValidInfo[]>();
      _modules = modules.ToDictionary(module => module.RuleId);
      Configuration = configuration;
   }

   public PropertyValidInfo[] GetPropertiesValidInfoFromType(Type type) {
      return _cachedProperties.GetOrAdd(type, t => {
         var props = t
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.IsDefined(typeof(ValidRuleAttribute), false)).ToArray();
         var result = new PropertyValidInfo[props.Length];
         for (var i = 0; i < props.Length; i++) {
            var prop = props[i];
            var attributes = prop.GetCustomAttributes<ValidRuleAttribute>().ToArray();
            var rules = new string[attributes.Length];
            var options = new string[attributes.Length];
            for (var y = 0; y < attributes.Length; y++) {
               var split = attributes[y].Instruction.Split("~");
               rules[y] = split[0];
               options[y] = split.Length == 1 ? string.Empty : split[1];
            }

            var param = Expression.Parameter(typeof(object));
            var getter = Expression.Lambda<Func<object, object?>>(
               Expression.Convert(
                  Expression.Property(Expression.Convert(param, type), prop),
                  typeof(object)),
               param).Compile();
            result[i] = new PropertyValidInfo(rules, options, getter);
         }

         return result;
      });
   }

   public async Task<Option<T>> ValidFromRequest<T>(Request request, Response response,
      CancellationToken cancellationToken)
      where T : class {
      var readOption = await request.ReadContentT<T>(cancellationToken);
      if (!readOption.Success) {
         response.PushError("Data struct is not correct");
         return Option<T>.Fail();
      }

      var validOption = await Valid(readOption.Content, request, response, cancellationToken);
      return new Option<T>(validOption, readOption.Content);
   }

   public async Task<bool> Valid<T>(T? instance, Request request, Response response,
      CancellationToken cancellationToken) {
      if (instance == null) {
         response.PushError("Data is null object", 400);
         return false;
      }

      foreach (var prop in GetPropertiesValidInfoFromType(instance.GetType()))
         for (var i = 0; i < prop.ArrayOptions.Length; i++) {
            var rule = prop.ArrayRules[i];
            var module = _modules[rule];
            var result = await module.Valid(prop.Getter(instance), prop.ArrayOptions[i], request, response,
               cancellationToken);
            if (result) continue;
            response.PushError(module.Message, 400);
            return false;
         }

      return true;
   }
}

// public sealed partial class ValidatorManager : IValidatorManager {
//    private const string CLOUDFLARE_TURNSTILE = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
//
//    private static string GetMessage(ValidRuleType rule, int value) {
//       return rule switch {
//          ValidRuleType.Need => "This field is required!",
//          ValidRuleType.Number => "The field is not a number",
//          ValidRuleType.Min => $"The field must be greater than {value}",
//          ValidRuleType.Max => $"The field must be less than {value}",
//          ValidRuleType.MinLength => $"The number of characters must be greater than {value}",
//          ValidRuleType.MaxLength => $"The number of characters must be less than {value}",
//          ValidRuleType.CloudflareCaptcha => $"Captcha verification failed",
//          ValidRuleType.Email => $"Invalid email address",
//          ValidRuleType.Phone => $"Invalid phone number",
//          ValidRuleType.DatePast => "The date is in the past",
//          _ => "Unknown field error"
//       };
//    }
//
//    private static async Task<bool> Check(ValidRuleType rule, int value, object? content,
//       CancellationToken cancellationToken) {
//       return content != null && rule switch {
//          ValidRuleType.Need => true,
//          ValidRuleType.Number => await VerifyNumber($"{content}"),
//          ValidRuleType.Min => content is float x && x >= value,
//          ValidRuleType.Max => content is float x && x <= value,
//          ValidRuleType.MinLength => $"{content}".Length >= value,
//          ValidRuleType.MaxLength => $"{content}".Length <= value,
//          ValidRuleType.CloudflareCaptcha => await VerifyCaptcha($"{content}", cancellationToken),
//          ValidRuleType.Email => await VerifyEmail($"{content}"),
//          ValidRuleType.Phone => await VerifyPhone($"{content}"),
//          ValidRuleType.DatePast => content is DateTime time && time > DateTime.Now,
//          _ => false
//       };
//    }
//
//    public async Task<bool> Valid<T>(T? instance, Request request, Response response,
//       CancellationToken cancellationToken) {
//       if (instance != null) {
//          var props = instance.GetType()
//             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//             .Where(x => x.IsDefined(typeof(ValidRuleAttribute), false));
//          foreach (var prop in props)
//          foreach (var attr in prop.GetCustomAttributes<ValidRuleAttribute>()) {
//             var check = await Check(attr.Type, attr.Value, prop.GetValue(instance), cancellationToken);
//             if (check)
//                continue;
//             response.ErrorStack.PushStack(GetMessage(attr.Type, attr.Value));
//             return false;
//          }
//
//          return true;
//       }
//
//       response.ErrorStack.PushStack("Data is empty");
//       return false;
//    }
//
//    private static ValueTask<bool> VerifyNumber(string value) {
//       return ValueTask.FromResult(float.TryParse(value, out _));
//    }
//
//    private static ValueTask<bool> VerifyEmail(string value) {
//       return ValueTask.FromResult(EmailRegex().IsMatch(value));
//    }
//
//    private static ValueTask<bool> VerifyPhone(string value) {
//       return ValueTask.FromResult(PhoneRegex().IsMatch(PhoneTrashRegex().Replace(value, "")));
//    }
//
//    private static async Task<bool> VerifyCaptcha(string value, CancellationToken cancellationToken) {
//       using var content = new FormUrlEncodedContent(new Dictionary<string, string> {
//          ["secret"] = WebApp.CaptchaSecureKey,
//          ["token"] = value
//       });
//       using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, CLOUDFLARE_TURNSTILE) {
//          Content = content
//       };
//       using var httpResponseMessage = await WebApp.SecureContextInstance.Client
//          .SendAsync(httpRequestMessage, cancellationToken)
//          .ConfigureAwait(false);
//       if (!httpResponseMessage.IsSuccessStatusCode) return false;
//       await using var stream =
//          await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
//       var result = await JsonSerializer
//          .DeserializeAsync(stream, CaptchaJsonContext.Default.CaptchaResponseModel, cancellationToken)
//          .ConfigureAwait(false);
//       return result?.Success ?? false;
//    }
//
//    [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
//       System.Text.RegularExpressions.RegexOptions.Singleline)]
//    private static partial System.Text.RegularExpressions.Regex EmailRegex();
//
//    [System.Text.RegularExpressions.GeneratedRegex(@"^\+[1-9]\d{1,14}$")]
//    private static partial System.Text.RegularExpressions.Regex PhoneRegex();
//
//    [System.Text.RegularExpressions.GeneratedRegex(@"[^\d+]")]
//    private static partial System.Text.RegularExpressions.Regex PhoneTrashRegex();
// }