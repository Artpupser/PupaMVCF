namespace PupaMVCF.Framework.Core.Extra;

public interface IHaveStack<T> {
   IReadOnlyList<T> StackContentList { get; }
   int LengthStack { get; }
   void PushStack(T v);
   T PopStack();
}