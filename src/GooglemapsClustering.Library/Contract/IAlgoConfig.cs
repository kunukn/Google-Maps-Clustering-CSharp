using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IAlgoConfig
	{
		bool DoShowGridLinesInGoogleMap { get; }
		int OuterGridExtend { get; }
		bool DoUpdateAllCentroidsToNearestContainingPoint { get; }
		bool DoMergeGridIfCentroidsAreCloseToEachOther { get; }
		double MergeWithin { get; }
		int MinClusterSize { get; }
		int MaxMarkersReturned { get; }
		int AlwaysClusteringEnabledWhenZoomLevelLess { get; }
		int ZoomlevelClusterStop { get; }
		int Gridx { get; }
		int Gridy { get; }
		HashSet<int> MarkerTypes { get; }
		int MaxPointsInCache { get; }
		string Environment { get; }
		int Threads { get; }
	}
}
