using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Tests;

public sealed class TestApp(
   IConfiguration configuration,
   IRouter router,
   ILogger<TestApp> logger)
   : WebApp(configuration, router, logger);