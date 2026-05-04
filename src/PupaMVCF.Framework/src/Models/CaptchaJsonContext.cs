using System.Text.Json.Serialization;

namespace PupaMVCF.Framework.Models;

[JsonSerializable(typeof(CaptchaResponseModel))]
internal partial class CaptchaJsonContext : JsonSerializerContext;