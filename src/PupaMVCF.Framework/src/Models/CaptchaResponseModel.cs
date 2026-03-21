using System.Text.Json.Serialization;

namespace PupaMVCF.Framework.Models;

public record CaptchaResponseModel {
   [JsonPropertyName("success")] public bool Success { get; init; }
   [JsonPropertyName("challenge_ts")] public DateTime ChallengeTime { get; init; } = DateTime.Now;
   [JsonPropertyName("hostname")] public string Hostname { get; init; } = string.Empty;
   [JsonPropertyName("action")] public string Action { get; init; } = string.Empty;
}