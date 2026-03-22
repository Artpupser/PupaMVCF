using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.ExampleProcess;

public sealed class ExampleApp : WebApp {
   public ExampleApp(IConfiguration configuration, IRouter router, ILogger<ExampleApp> logger) : base(
      configuration, router,
      logger) { }
}