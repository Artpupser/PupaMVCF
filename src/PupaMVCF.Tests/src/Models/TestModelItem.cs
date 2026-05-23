using System.Text.Json.Serialization;

using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Tests.Models;

public record TestModelItem {
   [ValidRule("need~")]
   [ValidRule("string_range~1 32")]
   [JsonPropertyName("name")]
   public string Name { get; init; } = string.Empty;

   [ValidRule("need~")]
   [ValidRule("number_range~1 100")]
   [JsonPropertyName("age")]
   public float Age { get; init; }

   [ValidRule("need~")]
   [ValidRule("string_range~1 255")]
   [ValidRule("email~")]
   [JsonPropertyName("email")]
   public string Email { get; init; } = string.Empty;
}