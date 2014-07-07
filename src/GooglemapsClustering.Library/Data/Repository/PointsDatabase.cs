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
		public readonly ThreadData ThreadData;

		private readonly object _lock = new object();

		public IList<P> GetPoints()
		{
			return ThreadData.AllPoints;
		}

		public IList<P>[] GetThreadPoints()
		{
			return ThreadData.ThreadPoints;
		}

		public ThreadData GetThreadData()
		{
			return this.ThreadData;
		}

		public int Threads
		{
			get { return ThreadData.Threads; }
		}

		public PointsDatabase(IMemCache memCache, string filepath, int threads)
		{			
			ThreadData = memCache.Get<ThreadData>(CacheKeys.PointsDatabase);
			if (ThreadData != null) return;	// cache hit

			lock (_lock)
			{
				// if 2nd threads gets here then it should be cache hit
				ThreadData = memCache.Get<ThreadData>(CacheKeys.PointsDatabase);
				if (ThreadData != null) return;

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

				if (points.Count > AlgoConfig.Get.MaxPointsInCache)
				{
					points = points.Take(AlgoConfig.Get.MaxPointsInCache).ToList();
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

				var llist = new LinkedList<P>();
				points.ForEach(p => llist.AddLast(p));

				ThreadData.AllPoints = points.AsReadOnly();

				if (ThreadData.Threads > 1)
				{
					// Thread related data
					// Data are partitioned evenly to be used by the individual threads 
					var delta = points.Count / Threads;

					// Divide all the points evenly to the array of pointlist
					for (int i = 0; i < Threads; i++)
					{
						ThreadData.ThreadPoints[i] = new List<P>();
						for (int j = 0; j < delta; j++)
						{
							var p = llist.First();
							llist.RemoveFirst();
							ThreadData.ThreadPoints[i].Add(p);
						}
					}

					// Add remaining points to last array
					while (llist.Any())
					{
						var p = llist.First();
						llist.RemoveFirst();
						ThreadData.ThreadPoints.Last().Add(p);
					}

					// Readonly array
					for (int i = 0; i < Threads; i++)
					{
						if (ThreadData.ThreadPoints[i] is List<P>)
						{
							ThreadData.ThreadPoints[i] = (ThreadData.ThreadPoints[i] as List<P>).AsReadOnly();
						}
					}
				}

				memCache.Add<ThreadData>(ThreadData, CacheKeys.PointsDatabase, TimeSpan.FromHours(24));

				sw.Stop();
				LoadTime = sw.Elapsed;
			}
		}
	}
}
