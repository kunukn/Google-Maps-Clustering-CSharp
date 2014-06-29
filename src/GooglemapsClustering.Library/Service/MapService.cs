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
using GooglemapsClustering.Clustering.Extensions;
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

				
				JsonMarkersReply reply;

				jsonReceive.Viewport.ValidateLatLon(); // Validate google map viewport input (should be always valid)
				jsonReceive.Viewport.Normalize();

				// Get all points from memory
				ThreadData threadData = _memoryDatabase.GetThreadData();

				#region fiter

				// Filter points
				threadData = FilterUtil.Filter(
					threadData,
					new FilterData { TypeFilterExclude = jsonReceive.TypeFilterExclude }
					);

				#endregion filter



				// Create new instance for every ajax request with input all points and json data
				ICluster clusterAlgo = new GridCluster(threadData, jsonReceive);

				var clusteringEnabled = jsonReceive.IsClusteringEnabled
					|| AlgoConfig.Get.AlwaysClusteringEnabledWhenZoomLevelLess > jsonReceive.Zoomlevel;

				// Clustering
				if (clusteringEnabled && jsonReceive.Zoomlevel < AlgoConfig.Get.ZoomlevelClusterStop)
				{
					#region cluster

					IList<P> markers = clusterAlgo.GetCluster(
						new ClusterInfo {ZoomLevel = jsonReceive.Zoomlevel}
						);

					#endregion cluster


					// Prepare data to the client
					reply = new JsonMarkersReply
					{
						Markers = markers,
						Polylines = clusterAlgo.GetPolyLines(),
					};

					// Return client data
					return reply;
				}


				// If we are here then there are no clustering
				// The number of items returned is restricted to avoid json data overflow
				IList<P> filteredDataset = ClusterAlgorithmBase.FilterDataset(threadData.AllPoints, jsonReceive.Viewport);
				IList<P> filteredDatasetMaxPoints = filteredDataset.Take(AlgoConfig.Get.MaxMarkersReturned).ToList();

				reply = new JsonMarkersReply
				{
					Markers = filteredDatasetMaxPoints,
					Polylines = clusterAlgo.GetPolyLines(),
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