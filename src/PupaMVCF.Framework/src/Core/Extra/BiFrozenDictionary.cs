using System.Collections;
using System.Collections.Frozen;

namespace PupaMVCF.Framework.Core.Extra;

public sealed class BiFrozenDictionary<K, V> : IEnumerable<(KeyValuePair<K, V>, KeyValuePair<V, K>)>,
   IEnumerator<(KeyValuePair<K, V>, KeyValuePair<V, K>)>
   where V : notnull
   where K : notnull {
   private readonly FrozenDictionary<K, V> _normal;
   private readonly FrozenDictionary<V, K> _reverse;
   private int _index;

   public BiFrozenDictionary(K[] keys, V[] values) {
      var length = keys.Length;
      if (length != values.Length)
         throw new ArgumentException("Keys and values not equals");
      var normal = new Dictionary<K, V>();
      var reverse = new Dictionary<V, K>();
      for (var i = 0; i < keys.Length; i++) {
         var key = keys[i];
         var value = values[i];
         normal.Add(key, value);
         reverse.Add(value, key);
      }

      _normal = normal.ToFrozenDictionary();
      _reverse = reverse.ToFrozenDictionary();
   }

   public K HardGetKey(V value) {
      return _reverse[value];
   }

   public V HardGetValue(K key) {
      return _normal[key];
   }

   public bool TryGetKey(V value, out K? result) {
      return _reverse.TryGetValue(value, out result);
   }

   public bool TryGetValue(K key, out V? result) {
      return _normal.TryGetValue(key, out result);
   }

   public IEnumerator<(KeyValuePair<K, V>, KeyValuePair<V, K>)> GetEnumerator() {
      return this;
   }

   IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
   }

   public bool MoveNext() {
      var value = _index + 1;
      _index = Math.Clamp(value, 0, _normal.Count - 1);
      return value == _normal.Count;
   }

   public void Reset() {
      _index = 0;
   }

   public (KeyValuePair<K, V>, KeyValuePair<V, K>) Current => (_normal.ElementAt(_index), _reverse.ElementAt(_index));

   object IEnumerator.Current => Current;

   public void Dispose() {
      throw new NotImplementedException();
   }
}