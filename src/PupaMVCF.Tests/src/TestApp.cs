using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Tests;

public sealed class TestApp(
   IConfiguration configuration,
   IRouter router,
   IValidatorManager validator,
   ILogger<TestApp> logger)
   : WebApp(configuration, router, validator,
      logger);