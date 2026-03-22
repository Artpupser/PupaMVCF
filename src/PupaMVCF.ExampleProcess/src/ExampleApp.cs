using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Routing;

namespace PupaMVCF.ExampleProcess;

public sealed class ExampleApp : WebApp {
   public ExampleApp(IConfiguration configuration, IRouter router,
      IDatabaseProcessor databaseProcessor, ILogger<ExampleApp> logger) : base(
      configuration, databaseProcessor, router,
      logger) { }
}