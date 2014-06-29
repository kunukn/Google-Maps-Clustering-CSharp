using System.Linq;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Config;

namespace GooglemapsClustering.Clustering.Utility
{
	public static class FilterUtil
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="threadData"></param>
		/// <param name="filterData"></param>
		/// <returns></returns>
		public static ThreadData Filter(ThreadData threadData, FilterData filterData)
		{
			if (filterData.TypeFilterExclude.Count == AlgoConfig.Get.MarkerTypes.Count)
			{
				// Filter all 
				return new ThreadData(threadData.Threads); // empty				
			}
			if (filterData.TypeFilterExclude.Any())
			{
				// Filter data by typeFilter value
				// Make new obj, don't overwrite obj data

				// todo remove this when using threads
				var td = new ThreadData(threadData.Threads)
				{
					AllPoints = threadData.AllPoints
						.Where(p => filterData.TypeFilterExclude.Contains(p.T) == false)
						.ToList()
				};

				// todo split up in threads usage, use threadData
				for (int i = 0; i < threadData.Threads; i++)
				{
					td.ThreadPoints[i] = threadData.ThreadPoints[i]
					.Where(p => filterData.TypeFilterExclude.Contains(p.T) == false)
					.ToList();
				}
				return td;
			}

			return threadData;
		}
	}
}
