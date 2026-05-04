using System.Collections.Frozen;
using System.Reflection;

namespace PupaMVCF.Framework.Routing;

public readonly struct RouteValue(Type controllerType, MethodInfo method, Type[] middlewareTypes) {
   public Type ControllerType { get; } = controllerType;
   public MethodInfo Method { get; } = method;
   public FrozenSet<Type> Middlewares { get; } = middlewareTypes.ToFrozenSet();

   public Queue<Type> ToQueueMiddlewareTypes() {
      return new Queue<Type>(Middlewares);
   }
}