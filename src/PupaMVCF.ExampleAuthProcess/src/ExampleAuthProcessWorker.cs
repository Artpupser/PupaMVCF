namespace PupaMVCF.ExampleAuthProcess;

public class ExampleAuthProcessWorker : BackgroundService {
   private readonly ExampleAuthServiceApp _app;
   private readonly ILogger<ExampleAuthProcessWorker> _logger;


   public ExampleAuthProcessWorker(ExampleAuthServiceApp app, ILogger<ExampleAuthProcessWorker> logger) {
      _app = app;
      _logger = logger;
   }

   protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
      _logger.LogInformation($"{nameof(ExampleAuthProcessWorker)} starting...");
      await _app.Run(cancellationToken);
   }

   public override async Task StopAsync(CancellationToken cancellationToken) {
      _logger.LogInformation($"{nameof(ExampleAuthProcessWorker)} stoping...");
      await _app.Stop(cancellationToken);
      await base.StopAsync(cancellationToken);
   }
}