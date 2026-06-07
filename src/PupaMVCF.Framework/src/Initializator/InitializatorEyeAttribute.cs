[AttributeUsage(AttributeTargets.Class)]
public sealed class InitializatorEyeAttribute(bool included) : Attribute {
   public bool Included {get;set;} = included;   
}
