using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IPointsDatabase
	{
		IList<P> GetPoints();
		IList<P>[] GetThreadPoints();
		ThreadData GetThreadData();
		int Threads { get; }	
	}
}
