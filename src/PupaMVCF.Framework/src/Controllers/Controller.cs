using System.Reflection;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Controllers;

public abstract class Controller {
   public delegate Task ControllerHandlerDelegate(Request request, Response response,
      CancellationToken cancellationToken);

   public IReadOnlyList<(ControllerHandlerDelegate Func, ControllerHandlerAttribute Attribute)> _handlers;

   protected Controller() {
      _handlers = GetType()
         .GetMethods(BindingFlags.Instance)
         .Where(x => x.IsDefined(typeof(ControllerHandlerAttribute), false))
         .Select(x => (Func: x.CreateDelegate<ControllerHandlerDelegate>(this),
            Attribute: x.GetCustomAttribute<ControllerHandlerAttribute>()!)).ToList();
   }
}