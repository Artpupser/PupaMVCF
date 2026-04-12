using System.Text.Json.Serialization;

using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Tests.Models;

public record TestModel {
   [ValidRule("need~")]
   [ValidRule("string_range~1 32")]
   [JsonPropertyName("id")]
   public string Id { get; set; }

   [ValidRule("need~")]
   [ValidRule("test_items~")]
   [JsonPropertyName("items")]
   public TestModelItem[] Items { get; set; }
}