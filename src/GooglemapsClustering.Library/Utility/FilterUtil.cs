using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		/// <param name="threadData"></param>
		/// <param name="filterData"></param>
		/// <returns></returns>
		public static ThreadData FilterByType(ThreadData threadData, FilterData filterData)
		{
			if (filterData.TypeFilterExclude.Count == GmcSettings.Get.MarkerTypes.Count)
			{
				// Filter all 
				return new ThreadData(threadData.Threads); // empty				
			}
			if (filterData.TypeFilterExclude.None())
			{
				// Filter none
				return threadData;
			}

			// Filter data by typeFilter value			
			return FilterThread<FilterData>(threadData, filterData, FilterByTypeHelper);
		}


		// O(n)
		public static ThreadData FilterDataByViewport(ThreadData threadData, Boundary viewport)
		{
			return FilterThread<Boundary>(threadData, viewport, FilterDataByViewportHelper);
		}

		// O(n)
		private static IList<P> FilterDataByViewportHelper(IList<P> points, Boundary viewport)
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

		
		/// <summary>
		/// O(n)
		/// Generic higher order filter function
		/// </summary>
		/// <typeparam name="F"></typeparam>
		/// <param name="threadData"></param>
		/// <param name="filterData"></param>
		/// <param name="filterMethod"></param>
		/// <returns></returns>
		private static ThreadData FilterThread<F>(ThreadData threadData, F filterData, Func<IList<P>, F, IList<P>> filterMethod)
		{
			var td = new ThreadData(threadData.Threads);

			// http://www.codeproject.com/Articles/189374/The-Basics-of-Task-Parallelism-via-C
			// http://www.codeproject.com/Articles/152765/Task-Parallel-Library-of-n
			if (GmcSettings.Get.Threads > 1)
			{
				var tasks = new Task<IList<P>>[GmcSettings.Get.Threads];
				for (int i = 0; i < tasks.Length; i++)
				{
					var ii = i;
					tasks[i] = new Task<IList<P>>(() => filterMethod.Invoke(threadData.ThreadPoints[ii], filterData));
				}

				// run the threads
				foreach (var t in tasks) t.Start();

				// wait until all done
				Task.WaitAll(tasks);
				
				// update data
				for (int i = 0; i < tasks.Length; i++) td[i] = tasks[i].Result;
			}
			else
			{				
				td[0] = filterMethod.Invoke(threadData[0], filterData);
			}

			return td;
		}

	}
}
