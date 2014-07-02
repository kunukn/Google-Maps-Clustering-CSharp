using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Utility
{
	public static class FilterUtil
	{
		/// <summary>
		///  Supports threads
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
			
				var td = new ThreadData(threadData.Threads)
				{
					// todo remove this when threads support is fully implemented
					AllPoints = FilterHelper( threadData.AllPoints, filterData)	 					
				};

				// http://www.codeproject.com/Articles/189374/The-Basics-of-Task-Parallelism-via-C
				// http://www.codeproject.com/Articles/152765/Task-Parallel-Library-of-n
				if (AlgoConfig.Get.Threads > 1)
				{
					var tasks = new Task<IList<P>>[AlgoConfig.Get.Threads];
					for (int i = 0; i < tasks.Length; i++)
					{
						var ii = i;
						tasks[i] = new Task<IList<P>>(() => FilterHelper(threadData.ThreadPoints[ii], filterData));
					}
					foreach (var t in tasks) t.Start();	// run the threads
					Task.WaitAll(tasks);

					for (int i = 0; i < tasks.Length; i++)
					{
						td.ThreadPoints[i] = tasks[i].Result;
					}								
				}
				
				return td;
			}

			return threadData;
		}

		private static IList<P> FilterHelper(IList<P> points, FilterData filterData)
		{
			return points
				.Where(p => filterData.TypeFilterExclude.Contains(p.T) == false)
				.ToList();
		}

	}
}
