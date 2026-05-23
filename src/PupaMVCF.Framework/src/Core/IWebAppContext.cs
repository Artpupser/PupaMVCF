namespace PupaMVCF.Framework.Core;

public interface IWebAppContext : IAnyAppContext<WebApp> {
   public HttpClient Client { get; }
}