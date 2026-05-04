namespace PupaMVCF.Framework.Routing;

public sealed class RouterMapBuilder {
   private readonly List<Type> _controllers = [];

   public RouterMapBuilder AddController<T>() {
      _controllers.Add(typeof(T));
      return this;
   }

   public IReadOnlyList<Type>
      Build() {
      return _controllers;
   }
}