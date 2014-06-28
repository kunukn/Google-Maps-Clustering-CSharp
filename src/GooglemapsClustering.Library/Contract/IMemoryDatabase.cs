using System.Collections.Generic;
using GooglemapsClustering.Clustering.Data;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IMemoryDatabase
	{
		IList<P> GetPoints();
	}
}
