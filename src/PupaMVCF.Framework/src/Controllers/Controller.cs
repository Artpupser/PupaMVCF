using System.Reflection;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

public abstract class Controller {
   public delegate Task ControllerHandlerDelegate(Request request, Response response,
      CancellationToken cancellationToken);

   public async Task ExecuteReflectionHandler(string pattern, Request request, Response response,
      CancellationToken cancellationToken) {
      var func = GetReflectionHandler(pattern);
      await func(request, response, cancellationToken);
   }

   public ControllerHandlerDelegate GetReflectionHandler(string pattern) {
      return GetReflectionHandler().First(x => x.Attribute!.Pattern == pattern).Func;
   }

   public IEnumerable<(ControllerHandlerDelegate Func, ControllerHandlerAttribute Attribute)> GetReflectionHandler() {
      return GetType()
         .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
         .Where(x => x.IsDefined(typeof(ControllerHandlerAttribute), false))
         .Select(x => (Func: x.CreateDelegate<ControllerHandlerDelegate>(this),
            Attribute: x.GetCustomAttribute<ControllerHandlerAttribute>()!));
   }
}