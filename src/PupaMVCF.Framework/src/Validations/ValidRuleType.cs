namespace PupaMVCF.Framework.Validations;

public enum ValidRuleType : byte {
   Need = 0,
   Number = 1,
   Min = 2,
   Max = 3,
   MinLength = 4,
   MaxLength = 5,
   CloudflareCaptcha = 6,
   Email = 7,
   Phone = 8,
   DatePast = 9
}