using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

public interface IMiddleware {
   public Task<Option> Invoke(Request request, Response response, CancellationToken cancellationToken);
}