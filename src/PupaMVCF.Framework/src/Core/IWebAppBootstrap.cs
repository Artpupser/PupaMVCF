namespace PupaMVCF.Framework.Core;

public interface IWebAppBootstrap {
   public Queue<Func<Task>> Operations();
}