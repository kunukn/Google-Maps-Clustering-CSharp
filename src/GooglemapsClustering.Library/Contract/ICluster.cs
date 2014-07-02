using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data.Algo;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface ICluster
	{
		IList<P> GetCluster(ClusterInfo clusterInfo);
		IList<Line> GetPolyLines();
	}
}
