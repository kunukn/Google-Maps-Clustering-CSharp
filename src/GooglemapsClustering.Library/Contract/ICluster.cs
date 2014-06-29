using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data.Algo;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface ICluster
	{
		List<P> GetCluster(ClusterInfo clusterInfo);
		List<Line> GetPolyLines();
	}
}
