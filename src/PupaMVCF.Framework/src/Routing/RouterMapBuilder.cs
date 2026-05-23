namespace PupaMVCF.Framework.Routing;

public sealed class RouterMapBuilder {
   private readonly List<Type> _controllers = [];

   public RouterMapBuilder AddController<T>() {
      _controllers.Add(typeof(T));
      return this;
   }

   public RouterMapBuilder AddController(Type type) {
      _controllers.Add(type);
      return this;
   }

   public RouterMapBuilder AddControllers(Type[] types) {
      foreach (var type in types) _controllers.Add(type);
      return this;
   }


   public IReadOnlyList<Type>
      Build() {
      return _controllers;
   }
}