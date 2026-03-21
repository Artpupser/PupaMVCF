using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public interface IRouter {
   public Task Execute(Request request, Response response, CancellationToken cancellationToken);
}