using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Generators;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Example;

public sealed class ExampleApp(
   IConfiguration configuration,
   JwtTokenGeneratorService jwtTokenGeneratorService,
   IRouter router,
   ILogger<ExampleApp> logger)
   : WebApp(configuration, jwtTokenGeneratorService, router,
      logger);