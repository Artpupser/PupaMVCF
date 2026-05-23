using HttpMethodType = PupaMVCF.Framework.Core.HttpMethodType;

namespace PupaMVCF.Framework.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ControllerHandlerAttribute(string pattern, HttpMethodType httpMethodType, params Type[] middlewares)
   : Attribute {
   public string Pattern { get; } = pattern;
   public HttpMethodType HttpMethodType { get; } = httpMethodType;
   public Type[] Middlewares { get; } = middlewares;
}