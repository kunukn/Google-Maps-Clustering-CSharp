using System.Collections.Generic;
using System.Linq;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Utility
{
	public static class FilterUtil
	{
		public static IList<P> Filter(IList<P> points, FilterData filterData)
		{
			if (filterData.TypeFilterExclude.Count == AlgoConfig.Get.MarkerTypes.Count)
			{
				// Filter all 
				return new List<P>(); // empty
			}
			if (filterData.TypeFilterExclude.Any())
			{
				// Filter data by typeFilter value
				// Make new obj, don't overwrite obj data
				return points
					.Where(p => filterData.TypeFilterExclude.Contains(p.T) == false)
					.ToList();
			}

			return points;
		}
	}
}
