using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Data
{
	public class ThreadData
	{
		public int Threads { get; private set; }
		public IList<P> AllPoints { get; set; }
		public IList<P>[] ThreadPoints { get; set; }
		public ThreadData(int threads)
		{
			Threads = threads;
			Init();
		}
		private void Init()
		{
			AllPoints = new List<P>();
			ThreadPoints = new IList<P>[Threads];
			for (int i = 0; i < Threads; i++) ThreadPoints[i] = new List<P>();
		}
	}
}
