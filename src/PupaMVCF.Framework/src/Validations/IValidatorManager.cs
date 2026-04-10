using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Validations;

public interface IValidatorManager {
   public Task<bool> Valid<T>(T? instance, Request request, Response response,
      CancellationToken cancellationToken);

   public Task<Option<T>> ValidFromRequest<T>(Request request, Response response, CancellationToken cancellationToken)
      where T : class;
}