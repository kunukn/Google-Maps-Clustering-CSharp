using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Extensions;

namespace GooglemapsClustering.Clustering.Data.Repository
{
	/// <summary>
	/// The database for all the existing points
	/// </summary>
	public class PointsDatabase : IPointsDatabase
	{

		public readonly string FilePath;
		public readonly TimeSpan LoadTime;

		private ThreadData ThreadData { get; set; }

		private readonly IMemCache _memCache;

		protected IList<P> Points { get; set; }

		protected readonly object _threadsafe = new object();

		public ThreadData GetThreadData()
		{
			return ThreadData;
		}

		public PointsDatabase(IMemCache memCache, string filepath, int threads)
		{
			_memCache = memCache;

			ThreadData = _memCache.Get<ThreadData>(CacheKeys.PointsDatabase);
			if (ThreadData != null) return;	// cache hit

			lock (_threadsafe)
			{
				// if 2nd threads gets here then it should be cache hit
				ThreadData = _memCache.Get<ThreadData>(CacheKeys.PointsDatabase);
				if (ThreadData != null) return;
			
				Points = _memCache.Get<IList<P>>(CacheKeys.PointsDatabase);
				if (Points != null) return;

				var sw = new Stopwatch();
				sw.Start();

				ThreadData = new ThreadData(threads);
				FilePath = filepath;

				// Load from file
				List<P> points = Utility.Dataset.LoadDataset(FilePath);
				if (points.None())
				{
					throw new Exception(string.Format("Data was not loaded from file: {0}", FilePath));
				}

				if (points.Count > GmcSettings.Get.MaxPointsInCache)
				{
					points = points.Take(GmcSettings.Get.MaxPointsInCache).ToList();
				}

				// Not important, can be deleted, only for ensuring visual randomness of marker display 
				// when not all can be displayed on screen
				//
				// Randomize order, when limit take is used for max marker display
				// random locations are selected
				// http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
				var rand = new Random();
				var c = points.Count;
				for (var i = 0; i < c; i++)
				{
					P temp = points[i];
					int r = rand.Next(c);
					points[i] = points[r];
					points[r] = temp;
				}

				var linkedlist = new LinkedList<P>();
				points.ForEach(p => linkedlist.AddLast(p));

				if (ThreadData.Threads > 1)
				{
					// Thread related data
					// Data are partitioned evenly to be used by the individual threads 
					var delta = points.Count / ThreadData.Threads;

					// Divide all the points evenly to the array of pointlist
					for (int i = 0; i < ThreadData.Threads; i++)
					{
						ThreadData.ThreadPoints[i] = new List<P>();
						for (int j = 0; j < delta; j++)
						{
							var p = linkedlist.First();
							linkedlist.RemoveFirst();
							ThreadData.ThreadPoints[i].Add(p);
						}
					}

					// Add remaining points to last array
					while (linkedlist.Any())
					{
						var p = linkedlist.First();
						linkedlist.RemoveFirst();
						ThreadData.ThreadPoints.Last().Add(p);
					}
					
					for (int i = 0; i < ThreadData.Threads; i++)
					{
						if (ThreadData.ThreadPoints[i] is List<P>)
						{
							// Readonly array
							//ThreadData.ThreadPoints[i] = (ThreadData.ThreadPoints[i] as List<P>).AsReadOnly();
						}
					}
				}
				else
				{
					ThreadData.ThreadPoints[0] = points; //.AsReadOnly();
				}
				var linkedList = new LinkedList<P>();
				points.ForEach(p => linkedList.AddLast(p));

				Points = points;

				_memCache.Set<ThreadData>(ThreadData, CacheKeys.PointsDatabase, TimeSpan.FromHours(24));

				var data = _memCache.Get<ThreadData>(CacheKeys.PointsDatabase);

				if (data == null)
				{
					throw new Exception("cache not working");
				}

				sw.Stop();
				LoadTime = sw.Elapsed;
			}
		}
	}
}
