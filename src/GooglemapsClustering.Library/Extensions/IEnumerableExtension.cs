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
	}
}