using System;
using System.Collections.Generic;

namespace RedisSqlCache.Common.Collections
{
    public static class CollectionExtMethods
    {
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            Dictionary<TKey, TElement> results = new Dictionary<TKey, TElement>();
            foreach (var item in source)
            {
                var selectedKey = keySelector(item);
                var selectedElement = elementSelector(item);
                if (selectedKey != null)
                {
                    results.Add(selectedKey, selectedElement);
                }
            }
            return results;
        }

        public static T[] ToArray<T>(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return new Buffer<T>(source).ToArray();
        }
    }
}
