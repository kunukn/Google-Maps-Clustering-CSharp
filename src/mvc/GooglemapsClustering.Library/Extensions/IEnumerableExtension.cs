using System;
using System.Collections.Generic;
using System.Linq;

namespace GooglemapsClustering.Clustering.Extensions
{
    public static class IEnumerableExtension
    {
        public static bool HasAny<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Array.ForEach(enumerable.ToArray(), action);
        }
    }
}