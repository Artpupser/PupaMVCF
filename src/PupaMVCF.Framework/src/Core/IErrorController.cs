namespace PupaMVCF.Framework.Core;

public interface IErrorController {
   IEnumerable<string> Errors { get; }
   void PushError(string message);
   void PushError(int status);
   void PushError(string message, int status);
}