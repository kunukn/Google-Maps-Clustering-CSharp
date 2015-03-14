using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IPointsDatabase
	{		
		IList<P> GetPoints();			
	}
}
