using System.Text.Json.Serialization;

using PupaMVCF.Framework.Models;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess.Models;

public record FruitModel() {
   [ValidRule("need~")]
   [ValidRule("string_range~1 30")]
   [JsonPropertyName("name")]
   public string Name { get; set; }

   [ValidRule("need~")]
   [ValidRule("number_range~1 64")]
   [JsonPropertyName("amount")]
   public float Amount { get; set; }
}