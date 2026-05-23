using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Core;

namespace PupaMVCF.Template;

public sealed class TemplateBootstrap(ILogger<TemplateBootstrap> logger) : IWebAppBootstrap {
   public Queue<Func<Task>> Operations() {
      var queue = new Queue<Func<Task>>();
      queue.Enqueue(TempplateOperation);
      return queue;
   }

   private async Task TempplateOperation() {
      logger.LogInformation("Boooo");
   }
}