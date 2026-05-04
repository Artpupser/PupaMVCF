using System.Text.Json.Serialization;

using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Tests.Models;

public record TestModelItem {
   [ValidRule("need~")]
   [ValidRule("string_range~1 32")]
   [JsonPropertyName("name")]
   public string Name { get; set; }

   [ValidRule("need~")]
   [ValidRule("number_range~1 100")]
   [JsonPropertyName("age")]
   public float Age { get; set; }

   [ValidRule("need~")]
   [ValidRule("string_range~1 255")]
   [ValidRule("email~")]
   [JsonPropertyName("email")]
   public string Email { get; set; }
}