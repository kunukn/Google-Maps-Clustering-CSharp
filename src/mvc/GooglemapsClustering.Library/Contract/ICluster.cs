using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface ICluster
	{
		IList<P> RunCluster();
		IList<Line> GetPolyLines(); // Google Maps debug lines
	}
}
