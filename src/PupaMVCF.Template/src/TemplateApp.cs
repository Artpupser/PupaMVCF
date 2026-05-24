using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Template;

public sealed class TemplateApp(
   IConfiguration configuration,
   JwtTokenGeneratorService jwtTokenGeneratorService, IRouter router,
   ILogger<TemplateApp> logger)
   : WebApp(configuration, jwtTokenGeneratorService, router, logger);