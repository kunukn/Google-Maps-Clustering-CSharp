using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Activation;
using Kunukn.GooglemapsClustering.Clustering.Algorithm;
using Kunukn.GooglemapsClustering.Clustering.Contract;
using Kunukn.GooglemapsClustering.Clustering.Data;
using Kunukn.GooglemapsClustering.Clustering.Data.Json;
using Kunukn.GooglemapsClustering.Clustering.Utility;
using Kunukn.SingleDetectLibrary.Code.Data;
using P = Kunukn.GooglemapsClustering.Clustering.Data.P;
using Points = Kunukn.GooglemapsClustering.Clustering.Data.Points;
using IPoints = Kunukn.GooglemapsClustering.Clustering.Contract.IPoints;

using IKnnAlgorithm = Kunukn.SingleDetectLibrary.Code.Contract.IAlgorithm;
using PDist = Kunukn.SingleDetectLibrary.Code.Data.PDist;

namespace Kunukn.GooglemapsClustering.Clustering.WebService
{
    [AspNetCompatibilityRequirements(RequirementsMode
        = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AjaxService : IAjaxService
    {

        long Sw(Stopwatch sw)
        {
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        static class Ajax
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

            // knn
            public const string lat = "lat";
            public const string lon = "lon";
            public const string k = "k";
            public const string dist = "dist";
            public const string type = "type";

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

            public static readonly HashSet<string> KnnReq = new HashSet<string>
                                                     {
                                                         lat,
                                                         lon,
                                                         k,
                                                     };
        }

        #region Post

        // Post
        public JsonMarkersReply Markers(
            double nelat, double nelon, double swlat, double swlon,
            int zoomlevel, string filter, int sendid
            )
        {
            var sw = new Stopwatch();
            sw.Start();

            var jsonReceive = new JsonGetMarkersReceive(nelat, nelon, swlat, swlon, zoomlevel, filter, sendid);

            var clusteringEnabled = jsonReceive.IsClusteringEnabled || AlgoConfig.AlwaysClusteringEnabledWhenZoomLevelLess > jsonReceive.Zoomlevel;

            JsonMarkersReply reply;

            jsonReceive.Viewport.ValidateLatLon(); // Validate google map viewport input (is always valid)
            jsonReceive.Viewport.Normalize();

            // Get all points from memory
            IPoints points = MemoryDatabase.GetPoints();

            if (jsonReceive.TypeFilterExclude.Count == AlgoConfig.MarkerTypes.Count)
            {
                // Filter all 
                points = new Points(); // empty
            }
            else if (jsonReceive.TypeFilterExclude.Count > 0)
            {
                // Filter data by typeFilter value
                // Make new obj, don't overwrite obj data
                points = new Points
                              {
                                  Data = points.Data
                                  .Where(p => jsonReceive.TypeFilterExclude.Contains(p.T) == false)
                                  .ToList()
                              };
            }

            // Create new instance for every ajax request with input all points and json data
            var clusterAlgo = new GridCluster(points, jsonReceive); // create polylines
                     
            // Clustering
            if (clusteringEnabled && jsonReceive.Zoomlevel < AlgoConfig.ZoomlevelClusterStop)
            {
                // Calculate data to be displayed
                var clusterPoints = clusterAlgo.GetCluster(new ClusterInfo
                                                               {
                                                                   ZoomLevel = jsonReceive.Zoomlevel,                                                                   
                                                               });                
                                
                var converted = DataConvert(clusterPoints);
                                
                // Prepare data to the client
                reply = new JsonMarkersReply
                            {
                                Markers = converted,
                                Rid = sendid,
                                Polylines = clusterAlgo.Lines,
                                Msec = Sw(sw),
                            };

                // Return client data
                return reply;
            }

            // If we are here then there are no clustering
            // The number of items returned is restricted to avoid json data overflow
            IPoints filteredDataset = ClusterAlgorithmBase.FilterDataset(points, jsonReceive.Viewport);
            IPoints filteredDatasetMaxPoints = new Points
                                                   {
                                                       Data = filteredDataset.Data
                                                       .Take(AlgoConfig.MaxMarkersReturned)
                                                       .ToList()
                                                   };

            reply = new JsonMarkersReply
                        {
                            Markers = DataConvert(filteredDatasetMaxPoints),
                            Rid = sendid,
                            Polylines = clusterAlgo.Lines,
                            Mia = filteredDataset.Count - filteredDatasetMaxPoints.Count,
                            Msec = Sw(sw),
                        };
            return reply;
        }

      

        // Post
        public JsonMarkerInfoReply MarkerInfo(string id, int sendid)
        {
            var sw = new Stopwatch();
            sw.Start();

            var uid = int.Parse(id);

            var marker = MemoryDatabase.GetPoints().Data.SingleOrDefault(i => i.I == uid);
            if(marker==null)
            {                
                return new JsonMarkerInfoReply
                {
                    Id = id,
                    Content = "Marker could not be found",
                    Rid = sendid,
                    Msec = Sw(sw)
                };
            }

            var reply = new JsonMarkerInfoReply
                            {
                                Rid = sendid,                                
                            };
            reply.BuildContent(marker);

            reply.Msec = Sw(sw);
            return reply;
        }

        #endregion Post
       
        #region Get        

        // Get
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
                var kv = a.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length != 2) continue;

                nvc.Add(kv[0], kv[1]);
            }

            foreach (var key in Ajax.MarkersReq)
            {
                if (nvc[key]!=null) continue;

                invalid.EMsg = string.Format("param {0} is missing",key);
                return invalid;
            }
            
            try
            {                                
                var nelat = nvc[Ajax.nelat].Replace("_", ".").ToDouble();
                var nelon = nvc[Ajax.nelon].Replace("_", ".").ToDouble();
                var swlat = nvc[Ajax.swlat].Replace("_", ".").ToDouble();
                var swlon = nvc[Ajax.swlon].Replace("_", ".").ToDouble();
                var zoomlevel = int.Parse(nvc[Ajax.zoom]);
                               
                var filter = nvc[Ajax.filter] ?? "";
                var sendid = nvc[Ajax.sid] == null ? 1 : int.Parse(nvc[Ajax.sid]);
                               
                // values are validated there
                return Markers(nelat, nelon, swlat, swlon, zoomlevel,
                    filter, sendid);
            }
            catch (Exception ex)
            {
                invalid.EMsg = string.Format("Parsing error param: {0}",
                    ex.Message);
            }

            return invalid;
        }

        // Get
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
                var kv = a.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length != 2) continue;

                nvc.Add(kv[0], kv[1]);
            }

            foreach (var key in Ajax.MarkerInfoReq)
            {
                if (nvc[key]!=null) continue;

                invalid.EMsg = string.Format("param {0} is missing",key);
                return invalid;
            }
            
                        
            try
            {   
                var id = nvc[Ajax.id];
                var sid = nvc[Ajax.sid] == null ? 1 : int.Parse(nvc[Ajax.sid]);

                // values are validated there
                return MarkerInfo(id,sid);
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
                Points = DataConvert(new Points
                                         {
                                             Data = MemoryDatabase.GetPoints().Data.Take(3).ToList()
                                         })
            };
            return reply;
        }

        
        // Example of usage:
        // Get 3 nearest ->             /AreaGMC/gmc.svc/Knn/lat=8_5;lon=10_25;k=3
        // Get 3 nearest type 1 ->      /AreaGMC/gmc.svc/Knn/lat=8_5;lon=10_25;k=3;type=1
        // Get nearest within 1 km ->   /AreaGMC/gmc.svc/Knn/lat=8_5;lon=10_25;k=1000000;dist=1
        public JsonKnnReply Knn(string s)
        {
            var sw = new Stopwatch();
            sw.Start();

            var invalid = new JsonKnnReply {};
            if (string.IsNullOrEmpty(s))
            {
                invalid.EMsg = "param is empty";
                return invalid;
            }

            var arr = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 3)
            {
                invalid.EMsg = string.Format("param length is not valid: {0}",arr.Length);
                return invalid;
            }

            var nvc = new NameValueCollection();
            foreach (var a in arr)
            {
                var kv = a.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length != 2) continue;

                nvc.Add(kv[0], kv[1]);
            }

            foreach (var key in Ajax.KnnReq)
            {
                if (nvc[key] != null) continue;

                invalid.EMsg = string.Format("param {0} is missing", key);
                return invalid;
            }

            try
            {
                var y = nvc[Ajax.lat].Replace("_", ".").ToDouble();
                var x = nvc[Ajax.lon].Replace("_", ".").ToDouble();
                var k = int.Parse(nvc[Ajax.k]);
                var type = nvc[Ajax.type] == null ? -1 : int.Parse(nvc[Ajax.type]);

                double? dist = null;
                if(! string.IsNullOrEmpty(nvc[Ajax.dist])) dist = nvc[Ajax.dist].Replace("_", ".").ToDouble();
                               
                // knn algo
                var algo = MemoryDatabase.Data as IKnnAlgorithm;
                if (algo == null)
                {
                    invalid.EMsg = "algorithm is not available";
                    return invalid;
                }

                // Use algo
                var origin = new SingleDetectLibrary.Code.Data.P { X = x, Y = y, Type = type };
                var knnSameTypeOnly = type != -1;

                var conf = new KnnConfiguration {K = k, SameTypeOnly = knnSameTypeOnly, MaxDistance = dist};                                
                var duration = algo.UpdateKnn(origin, conf);

                var nns = algo.Knn.NNs.Select(p => p as PDist).ToList();
                var gmsNns = new List<GmcPDist>();
                foreach (var i in nns)
                {
                    //i.Distance = Math.Round(i.Distance, 7);
                    var pdist = new GmcPDist
                                    {
                                        Id = i.Point.Uid.ToString(),
                                        Point = i.Point, 
                                        Distance = Math.Round(i.Distance, 7)
                                    };
                    gmsNns.Add(pdist);
                }
                
                var result = 
                new JsonKnnReply
                {
                    Data = string.Format("Distance in km, x: {0}; y: {1}; k: {2}; sameTypeOnly: {3}, algo msec: {4}",
                        x.DoubleToString(), y.DoubleToString(), k, knnSameTypeOnly, duration),
                    Nns = gmsNns, // cannot be interface, thus casting
                    
                    Msec = Sw(sw),
                };

                return result;
            }
            catch (Exception ex)
            {
                invalid.EMsg = string.Format("Parsing error param: {0}",
                    ex.Message);
                return invalid;
            }                                                         
        }

        #endregion Get

        /// <summary>
        /// Solve serializing to Json issue (Cannot be interface type)
        /// Use replace or use your own P type as you like 
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        protected static IList<P> DataConvert(IPoints ps)
        {
            return ps.Data.Select(p => p as P).ToList();
        }
    }
}