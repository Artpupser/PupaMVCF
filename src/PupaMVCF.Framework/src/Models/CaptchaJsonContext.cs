using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace PupaMVCF.Framework.Models;

[JsonSerializable(typeof(CaptchaResponseModel))]
internal class RecaptchaJsonContext : JsonSerializerContext {
   public RecaptchaJsonContext(JsonSerializerOptions? options) : base(options) { }

   public override JsonTypeInfo? GetTypeInfo(Type type) {
      throw new NotImplementedException();
   }

   protected override JsonSerializerOptions? GeneratedSerializerOptions { get; }
}