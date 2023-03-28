using System;
using System.Collections.Generic;

namespace YABA.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<K, V>(this Dictionary<K, V> source, Dictionary<K, V> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }
    }
}
