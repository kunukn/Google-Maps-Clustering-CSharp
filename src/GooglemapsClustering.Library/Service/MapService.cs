using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using GooglemapsClustering.Clustering.Algorithm;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Json;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Clustering.Service
{
    public class MapService : IMapService
    {
        static string Sw(Stopwatch sw)
        {
            sw.Stop();
            return sw.Elapsed.ToString();
        }

        static class Protocol
        {
            // markers
            public const string nelat = "nelat";
            public const string nelon = "nelon";
            public const string swlat = "swlat";
            public const string swlon = "swlon";
            public const string zoom = "zoom";

            public const string filter = "filter";
            public const string sid = "sid";

            // markerInfo
            public const string id = "id";

            public static readonly HashSet<string> MarkersReq = new HashSet<string>
                                                     {
                                                         nelat,
                                                         nelon,
                                                         swlat,
                                                         swlon,
                                                         zoom,
                                                     };

            public static readonly HashSet<string> MarkerInfoReq = new HashSet<string>
                                                     {
                                                         id
                                                     };
        }
        
        

        
        public JsonMarkersReply GetMarkers(string s)
        {
            var invalid = new JsonMarkersReply { Ok = "0" };

            if (string.IsNullOrWhiteSpace(s))
            {
                invalid.EMsg = "params is empty";
                return invalid;
            }

            var arr = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 5)
            {
                invalid.EMsg = "params length incorrect";
                return invalid;
            }

            var nvc = new NameValueCollection();
            foreach (var a in arr)
            {
                var kv = a.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length != 2) continue;

                nvc.Add(kv[0], kv[1]);
            }

            foreach (var key in Protocol.MarkersReq)
            {
                if (nvc[key] != null) continue;

                invalid.EMsg = string.Format("param {0} is missing", key);
                return invalid;
            }

            try
            {
                var nelat = nvc[Protocol.nelat].Replace("_", ".").ToDouble();
                var nelon = nvc[Protocol.nelon].Replace("_", ".").ToDouble();
                var swlat = nvc[Protocol.swlat].Replace("_", ".").ToDouble();
                var swlon = nvc[Protocol.swlon].Replace("_", ".").ToDouble();
                var zoomlevel = int.Parse(nvc[Protocol.zoom]);

                var filter = nvc[Protocol.filter] ?? "";
                var sendid = nvc[Protocol.sid] == null ? 1 : int.Parse(nvc[Protocol.sid]);

                // values are validated there
                var sw = new Stopwatch();
                sw.Start();

                var jsonReceive = new JsonGetMarkersReceive(nelat, nelon, swlat, swlon, zoomlevel, filter, sendid);

                var clusteringEnabled = jsonReceive.IsClusteringEnabled || AlgoConfig.Get.AlwaysClusteringEnabledWhenZoomLevelLess > jsonReceive.Zoomlevel;

                JsonMarkersReply reply;

                jsonReceive.Viewport.ValidateLatLon(); // Validate google map viewport input (is always valid)
                jsonReceive.Viewport.Normalize();

                // Get all points from memory
                List<P> points = MemoryDatabase.GetPoints();

                if (jsonReceive.TypeFilterExclude.Count == AlgoConfig.Get.MarkerTypes.Count)
                {
                    // Filter all 
                    points = new List<P>(); // empty
                }
                else if (jsonReceive.TypeFilterExclude.Count > 0)
                {
                    // Filter data by typeFilter value
                    // Make new obj, don't overwrite obj data
                    points = points
                        .Where(p => jsonReceive.TypeFilterExclude.Contains(p.T) == false)
                        .ToList();

                }

                // Create new instance for every ajax request with input all points and json data
                var clusterAlgo = new GridCluster(points, jsonReceive); // create polylines

                // Clustering
                if (clusteringEnabled && jsonReceive.Zoomlevel < AlgoConfig.Get.ZoomlevelClusterStop)
                {
                    // Calculate data to be displayed
                    var clusterPoints = clusterAlgo.GetCluster(new ClusterInfo
                    {
                        ZoomLevel = jsonReceive.Zoomlevel,
                    });

                    // Prepare data to the client
                    reply = new JsonMarkersReply
                    {
                        Markers = clusterPoints,
                        Rid = sendid,
                        Polylines = clusterAlgo.Lines,
                        Msec = Sw(sw),
                    };

                    // Return client data
                    return reply;
                }

                // If we are here then there are no clustering
                // The number of items returned is restricted to avoid json data overflow
                List<P> filteredDataset = ClusterAlgorithmBase.FilterDataset(points, jsonReceive.Viewport);
                List<P> filteredDatasetMaxPoints = filteredDataset.Take(AlgoConfig.Get.MaxMarkersReturned).ToList();

                reply = new JsonMarkersReply
                {
                    Markers = filteredDatasetMaxPoints,
                    Rid = sendid,
                    Polylines = clusterAlgo.Lines,
                    Mia = filteredDataset.Count - filteredDatasetMaxPoints.Count,
                    Msec = Sw(sw),
                };
                return reply;                
            }
            catch (Exception ex)
            {
                invalid.EMsg = string.Format("Parsing error param: {0}",
                    ex.Message);
            }

            return invalid;
        }

        
        public JsonMarkerInfoReply GetMarkerInfo(string s)
        {
            var invalid = new JsonMarkerInfoReply { Ok = "0" };

            if (string.IsNullOrWhiteSpace(s))
            {
                invalid.EMsg = "params is empty";
                return invalid;
            }

            var arr = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 1) return invalid;

            var nvc = new NameValueCollection();
            foreach (var a in arr)
            {
                var kv = a.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length != 2) continue;

                nvc.Add(kv[0], kv[1]);
            }

            foreach (var key in Protocol.MarkerInfoReq)
            {
                if (nvc[key] != null) continue;

                invalid.EMsg = string.Format("param {0} is missing", key);
                return invalid;
            }


            try
            {
                var id = nvc[Protocol.id];
                var sid = nvc[Protocol.sid] == null ? 1 : int.Parse(nvc[Protocol.sid]);

                // values are validated there
                var sw = new Stopwatch();
                sw.Start();

                var uid = int.Parse(id);

                var marker = MemoryDatabase.GetPoints().SingleOrDefault(i => i.I == uid);
                if (marker == null)
                {
                    return new JsonMarkerInfoReply
                    {
                        Id = id,
                        Content = "Marker could not be found",
                        Rid = sid,
                        Msec = Sw(sw)
                    };
                }

                var reply = new JsonMarkerInfoReply { Rid = sid, };

                reply.BuildContent(marker);

                reply.Msec = Sw(sw);
                return reply;                
            }
            catch (Exception ex)
            {
                invalid.EMsg = string.Format("Parsing error param: {0}",
                    ex.Message);
            }

            return invalid;
        }


        public JsonInfoReply Info()
        {
            var reply = new JsonInfoReply
            {
                DbSize = MemoryDatabase.GetPoints().Count,
                Points = MemoryDatabase.GetPoints().Take(3).ToList()
            };
            return reply;
        } 
    }
}