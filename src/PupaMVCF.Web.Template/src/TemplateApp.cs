using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Web.Template;

public sealed class TemplateApp : WebApp {
   public TemplateApp(IConfiguration configuration, IRouter router, ILogger<TemplateApp> logger) : base(
      configuration, router,
      logger) { }
}