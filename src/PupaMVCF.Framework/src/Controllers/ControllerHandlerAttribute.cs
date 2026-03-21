using HttpMethodType = PupaMVCF.Framework.Core.HttpMethodType;

namespace PupaMVCF.Framework.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ControllerHandlerAttribute : Attribute {
   public string Pattern { get; }
   public HttpMethodType HttpMethodType { get; }
   public Type[] Middlewares { get; }

   public ControllerHandlerAttribute(string pattern, HttpMethodType httpMethodType, params Type[] middlewares) {
      Pattern = pattern;
      HttpMethodType = httpMethodType;
      Middlewares = middlewares;
   }
}