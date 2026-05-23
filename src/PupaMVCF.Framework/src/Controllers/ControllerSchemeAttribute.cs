namespace PupaMVCF.Framework.Controllers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ControllerSchemeAttribute(string patternPrefix) : Attribute {
   public string PatternPrefix { get; } = patternPrefix;
}