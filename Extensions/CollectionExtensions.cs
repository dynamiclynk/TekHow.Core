using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekHow.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T: ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        public static string ToFlatString(this IEnumerable<string> inCollection)
        {
            if (inCollection == null) { return null; }

            var sb = new StringBuilder();

            foreach(var s in inCollection)
            {
                sb.Append(s.ToUpper());
            }

            return sb.ToString();
        }

        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            var func = new Func<K, V, V>((oldkey, oldvalue) => value);
            lock (func)
            {
                dictionary.AddOrUpdate(key, value, func);
            }
        }

        public static string GetValue(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }

        public static object GetValue(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }
    }
}
