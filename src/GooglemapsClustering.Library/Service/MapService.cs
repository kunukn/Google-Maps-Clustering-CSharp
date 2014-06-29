using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GooglemapsClustering.Clustering.Algorithm;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Algo;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Data.Json;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Clustering.Service
{
	public class MapService : IMapService
	{
		private readonly IMemoryDatabase _memoryDatabase;
		private readonly int _threads;
		
		public MapService(IMemoryDatabase memoryDatabase)
		{
			_memoryDatabase = memoryDatabase;
			_threads = _memoryDatabase.Threads;			
		}

		public JsonMarkersReply GetMarkers(JsonGetMarkersInput input)
		{
			// Decorate with elapsed time
			var sw = new Stopwatch();
			sw.Start();
			var reply = GetMarkersHelper(input);
			sw.Stop();
			reply.Msec = sw.Elapsed.ToString();
			return reply;
		}
		public JsonMarkersReply GetMarkersHelper(JsonGetMarkersInput input)
		{
			var invalid = new JsonMarkersReply { Ok = "0" };
			try
			{
				var nelat = input.nelat.ToDouble();
				var nelon = input.nelon.ToDouble();
				var swlat = input.swlat.ToDouble();
				var swlon = input.swlon.ToDouble();
				var zoomLevel = int.Parse(input.zoomLevel);
				var filter = input.filter ?? "";
		
				// values are validated there
				var jsonReceive = new JsonGetMarkersReceive(nelat, nelon, swlat, swlon, zoomLevel, filter);

				var clusteringEnabled = jsonReceive.IsClusteringEnabled
					|| AlgoConfig.Get.AlwaysClusteringEnabledWhenZoomLevelLess > jsonReceive.Zoomlevel;

				JsonMarkersReply reply;

				jsonReceive.Viewport.ValidateLatLon(); // Validate google map viewport input (should be always valid)
				jsonReceive.Viewport.Normalize();

				// Get all points from memory
				IList<P> points = _memoryDatabase.GetPoints();
				ThreadData threadData = _memoryDatabase.GetThreadData();


				#region todo split up in threads usage, use  threadData

				points = FilterUtil.Filter(
					points, 
					new FilterData{TypeFilterExclude = jsonReceive.TypeFilterExclude}
					);
			
				#endregion todo split up in threads	usage


				// Create new instance for every ajax request with input all points and json data
				var clusterAlgo = new GridCluster(points, jsonReceive, threadData); 

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
						Polylines = clusterAlgo.Lines,
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
					Polylines = clusterAlgo.Lines,
					Mia = filteredDataset.Count - filteredDatasetMaxPoints.Count,
				};
				return reply;
			}
			catch (Exception ex)
			{
				invalid.EMsg = string.Format("MapService says: exception {0}",
					ex.Message);
				return invalid;
			}
		}


		public JsonMarkerInfoReply GetMarkerInfo(string input)
		{
			// Decorate with elapsed time
			var sw = new Stopwatch();
			sw.Start();
			var reply = GetMarkerInfoHelper(input);
			sw.Stop();
			reply.Msec = sw.Elapsed.ToString();
			return reply;
		}
		public JsonMarkerInfoReply GetMarkerInfoHelper(string id)
		{
			var invalid = new JsonMarkerInfoReply { Ok = "0" };

			if (string.IsNullOrWhiteSpace(id))
			{
				invalid.EMsg = "MapService says: params is invalid";
				return invalid;
			}
			try
			{
				var sw = new Stopwatch();
				sw.Start();

				var uid = int.Parse(id);

				var marker = _memoryDatabase.GetPoints().SingleOrDefault(i => i.I == uid); // O(n)
				if (marker == null)
				{
					return new JsonMarkerInfoReply
					{
						Id = id,
						Content = "Marker could not be found",
					};
				}

				var reply = new JsonMarkerInfoReply();
				reply.BuildContent(marker);
				return reply;
			}
			catch (Exception ex)
			{
				invalid.EMsg = string.Format("MapService says: Parsing error param: {0}",
					ex.Message);
			}

			return invalid;
		}


		public JsonInfoReply Info()
		{
			// Decorate with elapsed time
			var sw = new Stopwatch();
			sw.Start();
			var reply = InfoHelper();
			sw.Stop();
			reply.Msec = sw.Elapsed.ToString();
			return reply;
		}
		public JsonInfoReply InfoHelper()
		{
			return new JsonInfoReply
			{
				DbSize = _memoryDatabase.GetPoints().Count,
				FirstPoint = _memoryDatabase.GetPoints().FirstOrDefault()
			};

		}
	}
}