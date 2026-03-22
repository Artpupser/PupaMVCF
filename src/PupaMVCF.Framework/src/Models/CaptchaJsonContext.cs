using System.Text.Json.Serialization;
using System.Text.Json;

namespace PupaMVCF.Framework.Models;

[JsonSerializable(typeof(CaptchaResponseModel))]
internal partial class CaptchaJsonContext : JsonSerializerContext;