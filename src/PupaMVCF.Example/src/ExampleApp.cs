using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Example;

public sealed class ExampleApp(
   IConfiguration configuration,
   IRouter router,
   ILogger<ExampleApp> logger)
   : WebApp(configuration, router,
      logger);