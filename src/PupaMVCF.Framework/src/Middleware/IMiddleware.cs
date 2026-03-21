using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Middleware;

public interface IMiddleware {
   public Task<bool> Invoke(Request request, Response response, CancellationToken cancellationToken);
}