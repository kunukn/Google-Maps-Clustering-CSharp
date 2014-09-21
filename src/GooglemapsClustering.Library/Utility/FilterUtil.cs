using System.Collections.Generic;
using System.Linq;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Algo;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Extensions;

namespace GooglemapsClustering.Clustering.Utility
{
	public static class FilterUtil
	{
		/// <summary>
		///  Supports threads
		/// </summary>
		/// <param name="points"></param>
		/// <param name="filterData"></param>
		/// <returns></returns>
		public static IList<P> FilterByType(IList<P> points, FilterData filterData)
		{
			if (filterData.TypeFilterExclude.Count == GmcSettings.Get.MarkerTypes.Count)
			{
				// Filter all 
				return new List<P>(); // empty				
			}
			if (filterData.TypeFilterExclude.None())
			{
				// Filter none
				return points;
			}

			// Filter data by typeFilter value			
		    return FilterByTypeHelper(points, filterData);
		}


		// O(n)
        public static IList<P> FilterDataByViewport(IList<P> points, Boundary viewport)
		{
            return points
                 .Where(i => MathTool.IsInside(viewport, i))
                 .ToList();
		}		

		// O(n)
		private static IList<P> FilterByTypeHelper(IList<P> points, FilterData filterData)
		{
			return points
				.Where(p => filterData.TypeFilterExclude.Contains(p.T) == false)
				.ToList();
		}				
	}
}
