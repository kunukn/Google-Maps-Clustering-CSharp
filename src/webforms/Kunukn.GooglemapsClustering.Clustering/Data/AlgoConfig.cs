using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    public static class AlgoConfig
    {
        const string SectionLocal = "kunukn.GmcSettings/Local";
        const string SectionGlobal = "kunukn.GmcSettings/Global";
      
        static AlgoConfig()
        {
            var local = GetSectionLocal();
            var global = GetSectionGlobal();
            string s;

            Gridx = int.Parse(local[s = "Gridx"] ?? global[s] ?? Throw(s));
            Gridy = int.Parse(local[s = "Gridy"] ?? global[s] ?? Throw(s));
            DoShowGridLinesInGoogleMap = bool.Parse(local[s = "DoShowGridLinesInGoogleMap"] ?? global[s] ?? Throw(s) );
            OuterGridExtend = int.Parse(local[s = "OuterGridExtend"] ?? global[s] ?? Throw(s));                        
            DoUpdateAllCentroidsToNearestContainingPoint = bool.Parse(local[s = "DoUpdateAllCentroidsToNearestContainingPoint"] ?? global[s] ?? Throw(s));
            DoMergeGridIfCentroidsAreCloseToEachOther = bool.Parse(local[s = "DoMergeGridIfCentroidsAreCloseToEachOther"] ?? global[s] ?? Throw(s));
            MergeWithin = int.Parse(local[s = "MergeWithin"] ?? global[s] ?? Throw(s));
            MinClusterSize = int.Parse(local[s = "MinClusterSize"] ?? global[s] ?? Throw(s));
            MaxMarkersReturned = int.Parse(local[s = "MaxMarkersReturned"] ?? global[s] ?? Throw(s));
            AlwaysClusteringEnabledWhenZoomLevelLess = int.Parse(local[s = "AlwaysClusteringEnabledWhenZoomLevelLess"] ?? global[s] ?? Throw(s));
            ZoomlevelClusterStop = int.Parse(local[s = "ZoomlevelClusterStop"] ?? global[s] ?? Throw(s));
            
            Environment = local[s = "Environment"] ?? global[s] ?? Throw(s);
            PreClustered = bool.Parse(local[s = "PreClustered"] ?? global[s] ?? Throw(s));
            
            var types = (local[s = "MarkerTypes"] ?? global[s] ?? Throw(s)).Split(new []{";"},StringSplitOptions.RemoveEmptyEntries);
            MarkerTypes = new HashSet<int>();
            foreach (var type in types) MarkerTypes.Add(int.Parse(type));                        
        }

        // Use debug data        
        public static readonly bool DoShowGridLinesInGoogleMap; // generate draw grid lines info to google map

        // How much data that is send to client
        // EDIT to extend to widen or shorten gridview for outside view, must be minimum 0
        // default value is 1 which returns same data as illustrated in the picture from my blog
        // (see googlemaps-clustering-viewport_ver1.png inside the Docements/Design folder)
        public static readonly int OuterGridExtend;

        // Merge cluster points
        public static readonly bool DoUpdateAllCentroidsToNearestContainingPoint; // move centroid point to nearest existing point?
        public static readonly bool DoMergeGridIfCentroidsAreCloseToEachOther; // merge clusterpoints if close to each other?
        public static readonly double MergeWithin; // if neighbor cluster is within 1/n dist then merge, heuristic, higher value gives less merging

        // Cluster decision
        public static readonly int MinClusterSize; // only cluster if minimum this number of points

        // If clustering is disabled, restrict number of markers returned
        public static readonly int MaxMarkersReturned; 
        
        // Always cluster if equal or below this zoom level
        // to disable this effect set the value to -1
        public static readonly int AlwaysClusteringEnabledWhenZoomLevelLess;
        
        // Stop clustering from this zoom level and larger
        public static readonly int ZoomlevelClusterStop;

        
        // Pre cluster on app startup, NOT IMPLEMENTED YET
        public static readonly bool PreClustered;

        // Grid array
        public static readonly int Gridx;
        public static readonly int Gridy;

        // Array of existing marker types
        public static readonly HashSet<int> MarkerTypes;
                
        // 
        public static readonly string Environment;        

        // 
        public static NameValueCollection GetSectionLocal()
        {
            return (NameValueCollection)ConfigurationManager.GetSection(SectionLocal);
        }
        public static NameValueCollection GetSectionGlobal()
        {
            return (NameValueCollection)ConfigurationManager.GetSection(SectionGlobal);
        }

        static string Throw(string s)
        {            
            throw new NoNullAllowedException(s);            
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