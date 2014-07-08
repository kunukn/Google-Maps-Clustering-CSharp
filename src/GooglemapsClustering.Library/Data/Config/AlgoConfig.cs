using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using GooglemapsClustering.Clustering.Contract;

namespace GooglemapsClustering.Clustering.Data.Config
{
	/// <summary>
	/// Configurations data
	/// Readonly fields
	/// Data are parsed from a config file
	/// </summary>
	public class AlgoConfig : IAlgoConfig
	{
		const string SectionLocal = "kunukn.GmcSettings/Local";
		const string SectionGlobal = "kunukn.GmcSettings/Global";

		private static AlgoConfig _algoConfig;

		/// <summary>
		/// Singleton
		/// </summary>
		public static AlgoConfig Get
		{
			get
			{
				return _algoConfig ?? (_algoConfig = new AlgoConfig());
			}
		}

		private AlgoConfig()
		{
			var local = GetSectionLocal();
			var global = GetSectionGlobal();
			string s;

			Gridx = int.Parse(local[s = "Gridx"] ?? global[s] ?? Throw(s));
			Gridy = int.Parse(local[s = "Gridy"] ?? global[s] ?? Throw(s));
			DoShowGridLinesInGoogleMap = bool.Parse(local[s = "DoShowGridLinesInGoogleMap"] ?? global[s] ?? Throw(s));
			OuterGridExtend = int.Parse(local[s = "OuterGridExtend"] ?? global[s] ?? Throw(s));
			DoUpdateAllCentroidsToNearestContainingPoint = bool.Parse(local[s = "DoUpdateAllCentroidsToNearestContainingPoint"] ?? global[s] ?? Throw(s));
			DoMergeGridIfCentroidsAreCloseToEachOther = bool.Parse(local[s = "DoMergeGridIfCentroidsAreCloseToEachOther"] ?? global[s] ?? Throw(s));
			MergeWithin = int.Parse(local[s = "MergeWithin"] ?? global[s] ?? Throw(s));
			MinClusterSize = int.Parse(local[s = "MinClusterSize"] ?? global[s] ?? Throw(s));
			MaxMarkersReturned = int.Parse(local[s = "MaxMarkersReturned"] ?? global[s] ?? Throw(s));
			AlwaysClusteringEnabledWhenZoomLevelLess = int.Parse(local[s = "AlwaysClusteringEnabledWhenZoomLevelLess"] ?? global[s] ?? Throw(s));
			ZoomlevelClusterStop = int.Parse(local[s = "ZoomlevelClusterStop"] ?? global[s] ?? Throw(s));
			MaxPointsInCache = int.Parse(local[s = "MaxPointsInCache"] ?? global[s] ?? Throw(s));
			Threads = int.Parse(local[s = "Threads"] ?? global[s] ?? Throw(s));
			CacheServices = bool.Parse(local[s = "CacheServices"] ?? global[s] ?? Throw(s));

			Environment = local[s = "Environment"] ?? global[s] ?? Throw(s);
			

			var types = (local[s = "MarkerTypes"] ?? global[s] ?? Throw(s)).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			MarkerTypes = new HashSet<int>();
			foreach (var type in types) MarkerTypes.Add(int.Parse(type));
		}

		// Use debug data        
		public bool DoShowGridLinesInGoogleMap { get; private set; } // generate draw grid lines info to google map

		// How much data that is send to client
		// EDIT to extend to widen or shorten gridview for outside view, must be minimum 0
		// default value is 1 which returns same data as illustrated in the picture from my blog
		// (see googlemaps-clustering-viewport_ver1.png inside the Docements/Design folder)
		public int OuterGridExtend { get; private set; }

		// Merge cluster points
		public bool DoUpdateAllCentroidsToNearestContainingPoint { get; private set; } // move centroid point to nearest existing point?
		public bool DoMergeGridIfCentroidsAreCloseToEachOther { get; private set; } // merge clusterpoints if close to each other?
		public bool CacheServices { get; private set; } // cache get markers and get markers info services

		public double MergeWithin { get; private set; } // if neighbor cluster is within 1/n dist then merge, heuristic, higher value gives less merging

		// Cluster decision
		public int MinClusterSize { get; private set; }	 // only cluster if minimum this number of points

		// If clustering is disabled, restrict number of markers returned
		public int MaxMarkersReturned { get; private set; }

		// Always cluster if equal or below this zoom level
		// to disable this effect set the value to -1
		public int AlwaysClusteringEnabledWhenZoomLevelLess { get; private set; }

		// Stop clustering from this zoom level and larger
		public int ZoomlevelClusterStop { get; private set; }

		// Grid array
		public int Gridx { get; private set; }
		public int Gridy { get; private set; }

		// Array of existing marker types
		public HashSet<int> MarkerTypes { get; private set; }

		// Max allowed points in memory cache
		public int MaxPointsInCache { get; private set; }

		// 
		public string Environment { get; private set; }

		// Number of concurrent threads running for faster clustering
		public int Threads { get; private set; }

		// 
		public NameValueCollection GetSectionLocal()
		{
			return (NameValueCollection)ConfigurationManager.GetSection(SectionLocal);
		}
		public static NameValueCollection GetSectionGlobal()
		{
			return (NameValueCollection)ConfigurationManager.GetSection(SectionGlobal);
		}

		static string Throw(string s)
		{
			throw new Exception(string.Format("GmcGlobalKeySettings setup error: {0}", s));
		}

		public class EnvironmentOption
		{
			public const string Local = "local";
			public const string Development = "dev";
			public const string Test = "test";
			public const string Prod = "prod";
		}

	}
}