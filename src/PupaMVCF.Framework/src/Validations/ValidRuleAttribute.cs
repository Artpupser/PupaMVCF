namespace PupaMVCF.Framework.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class ValidRuleAttribute : Attribute {
   public ValidRuleType Type { get; init; }
   public int Value { get; init; }

   public ValidRuleAttribute(ValidRuleType type, int value = 0) {
      Type = type;
      Value = value;
   }
}