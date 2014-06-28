using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Data
{
	public class ThreadData
	{
		public int Threads { get; private set; }
		public IList<P> Points { get; set; }
		public IList<P>[] ThreadPoints { get; set; }
		public ThreadData(int threads)
		{
			Threads = threads;
		}
	
	}
}
