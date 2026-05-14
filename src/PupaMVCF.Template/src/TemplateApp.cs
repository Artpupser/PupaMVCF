using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Web.Template;

public sealed class TemplateApp(
   IConfiguration configuration,
   IValidatorManager validator,
   IRouter router,
   ILogger<TemplateApp> logger)
   : WebApp(configuration, router, validator,
      logger);