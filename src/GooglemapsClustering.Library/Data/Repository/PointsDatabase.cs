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
        protected readonly IMemCache _memCache;
		protected IList<P> Points { get; set; }

        protected readonly object _lock = new object();

		public IList<P> GetPoints()
		{
			return Points;
			//return _memCache.Get<ThreadData>(CacheKeys.PointsDatabase);
		}

		public PointsDatabase(IMemCache memCache, string filepath)
		{
			_memCache = memCache;

			Points = _memCache.Get<IList<P>>(CacheKeys.PointsDatabase);
			if (Points != null) return;	// cache hit

			lock (_lock)
			{
				// if 2nd threads gets here then it should be cache hit
                Points = _memCache.Get<IList<P>>(CacheKeys.PointsDatabase);
				if (Points != null) return;

				var sw = new Stopwatch();
				sw.Start();

				Points = new List<P>();
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

				var llist = new LinkedList<P>();
				points.ForEach(p => llist.AddLast(p));
							
				{
					Points = points;
				}

				_memCache.Set<IList<P>>(Points, CacheKeys.PointsDatabase, TimeSpan.FromHours(24));

                var data = _memCache.Get<IList<P>>(CacheKeys.PointsDatabase);

				if (data == null) throw new Exception("cache not working");

				sw.Stop();
				LoadTime = sw.Elapsed;
			}
		}
	}
}
