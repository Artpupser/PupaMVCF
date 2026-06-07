using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;

using PupaLib.Core;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations;

public sealed class ValidatorManager : IValidatorManager {
   public readonly IReadOnlyDictionary<string, ValidatorModule> Modules;
   private readonly ConcurrentDictionary<Type, PropertyValidInfo[]> _cachedProperties;
   private IConfiguration Configuration { get; }

   public ValidatorManager(IServiceProvider serviceProvider, IConfiguration configuration) {
      _cachedProperties = new ConcurrentDictionary<Type, PropertyValidInfo[]>();
      Modules = InitializatorBuilder
         .CreateInstances<ValidatorModule>(serviceProvider, [this]!)
         .ToDictionary(module => module.RuleId);
      Configuration = configuration;
   }

   public PropertyValidInfo[] GetPropertiesValidInfoFromType(Type type) {
      return _cachedProperties.GetOrAdd(type, t => {
         var props = t
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.IsDefined(typeof(ValidRuleAttribute), false))
            .ToArray();
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
            var module = Modules[rule];
            var result = await module.Valid(prop.Getter(instance), prop.ArrayOptions[i], request, response,
               cancellationToken);
            if (result) continue;
            response.PushError(module.Message, 400);
            return false;
         }

      return true;
   }
}

