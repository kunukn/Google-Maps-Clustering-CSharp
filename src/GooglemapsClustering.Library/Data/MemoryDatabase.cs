using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GooglemapsClustering.Clustering.Contract;

namespace GooglemapsClustering.Clustering.Data
{
	/// <summary>
	/// The database for all the existing points
	/// </summary>
	public class MemoryDatabase : IMemoryDatabase
	{
		public readonly string FilePath;
		public readonly TimeSpan LoadTime;
		public readonly IList<P> AllPoints;
		private readonly int _threads;


		public IList<P> GetPoints()
		{
			return AllPoints;
		}

		public int Threads
		{
			get { return _threads; }
		}

		public MemoryDatabase(string filepath, int threads)
		{
			var sw = new Stopwatch();
			sw.Start();

			FilePath = filepath;
			_threads = threads;			 

			// Load from file
			List<P> points = Utility.Dataset.LoadDataset(FilePath);
			if (!points.Any())
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
			var rand = new Random();
			var c = points.Count;
			for (var i = 0; i < c; i++)
			{
				//var p = points[i]; // do something with each p ?        
				var a = rand.Next(c);
				var b = rand.Next(c);
				var temp = points[a];
				points[a] = points[b];
				points[b] = temp;
			}

			AllPoints = points.AsReadOnly();

			sw.Stop();
			LoadTime = sw.Elapsed;
		}
	}
}
