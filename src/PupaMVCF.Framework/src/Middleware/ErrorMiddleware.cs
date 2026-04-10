using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

public sealed class ErrorMiddleware : IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken) {
      var i = 0;
      foreach (var error in response.Errors) {
         Console.WriteLine($"[{i}] {error}");
         i++;
      }

      return Option.OkTask();
   }
}