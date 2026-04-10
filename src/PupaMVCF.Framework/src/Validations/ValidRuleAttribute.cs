namespace PupaMVCF.Framework.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class ValidRuleAttribute : Attribute {
   /// <summary>
   /// Separator :~:, Example: number_range:~:0 15
   /// </summary>
   public string Instruction { get; }

   public ValidRuleAttribute(string instruction) {
      Instruction = instruction;
   }
}