namespace PupaMVCF.Framework.Validations;

public record PropertyValidInfo(string[] ArrayRules, string[] ArrayOptions, Func<object, object?> Getter);