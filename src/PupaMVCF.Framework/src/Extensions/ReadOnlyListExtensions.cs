namespace PupaMVCF.Framework.Extensions;

public static class ReadOnlyListExtensions {
   public static int IndexOf<T>(this IReadOnlyList<T> list, T item) {
      if (list is IList<T> ilist)
         return ilist.IndexOf(item);
      for (var i = 0; i < list.Count; i++)
         if (EqualityComparer<T>.Default.Equals(list[i], item))
            return i;
      return -1;
   }
}