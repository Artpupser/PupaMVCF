using System.Collections.Frozen;

using PupaMVCF.Framework.Controllers;

namespace PupaMVCF.Framework.Routing;

public readonly struct RouteValue {
   public Controller.ControllerHandlerDelegate Handler { get; }
   public FrozenSet<Type> Middlewares { get; }

   public RouteValue(Controller.ControllerHandlerDelegate handler, IEnumerable<Type> middlewares) {
      Handler = handler;
      Middlewares = middlewares.ToFrozenSet();
   }

   public Queue<Type> GetQueueMiddlewares() {
      return new Queue<Type>(Middlewares);
   }
}