using PupaMVCF.Framework.Validations;

namespace PupaMVCF.Framework.Core;

public interface IWebAppContext : IAnyAppContext<WebApp> {
   public HttpClient Client { get; }
   public IValidatorManager Validator { get; }
}