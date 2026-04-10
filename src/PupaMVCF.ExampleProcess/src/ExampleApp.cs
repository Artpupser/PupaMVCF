using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess;

public sealed class ExampleApp(
   IConfiguration configuration,
   IRouter router,
   IValidatorManager validator,
   ILogger<ExampleApp> logger)
   : WebApp(configuration, router, validator,
      logger);