using PupaMVCF.ExampleProcess.Components;

namespace PupaMVCF.ExampleProcess;

public class ExampleAppWorker : BackgroundService {
   private readonly ExampleApp _app;
   private readonly ILogger<ExampleAppWorker> _logger;

   public ExampleAppWorker(ExampleApp app, ILogger<ExampleAppWorker> logger) {
      _logger = logger;
      _app = app;
   }

   protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
      HeaderComponent.PreloadHeader([("Главная", "/"), ("О нас", "/aboutus")]);
      _logger.LogInformation($"{nameof(ExampleAppWorker)} starting...");
      await _app.Run(cancellationToken);
   }

   public override async Task StopAsync(CancellationToken cancellationToken) {
      _logger.LogInformation($"{nameof(ExampleAppWorker)} stoping...");
      await base.StopAsync(cancellationToken);
   }
}