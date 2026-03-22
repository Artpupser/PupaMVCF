namespace PupaMVCF.Framework.Core;

public interface ISecureWebAppContext : ISecureAnyAppContext<WebApp> {
   public HttpClient Client { get; }
}