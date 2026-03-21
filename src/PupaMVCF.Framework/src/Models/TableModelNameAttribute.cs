namespace PupaMVCF.Framework.Models;

[AttributeUsage(AttributeTargets.Class)]
public sealed class TableModelNameAttribute(string name) : Attribute {
   public string Name { get; } = name;
}