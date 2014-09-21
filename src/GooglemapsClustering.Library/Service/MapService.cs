using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GooglemapsClustering.Clustering.Algorithm;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Data.Json;
using GooglemapsClustering.Clustering.Extensions;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Clustering.Service
{
	public class MapService : IMapService
	{
		private readonly IPointsDatabase _pointsDatabase;
		private readonly IMemCache _memCache;		

		public MapService(IPointsDatabase pointsDatabase, IMemCache memCache)
		{
			_pointsDatabase = pointsDatabase;
			_memCache = memCache;			
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
		/// <summary>
		/// Read Through Cache
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public JsonMarkersReply GetMarkersHelper(JsonGetMarkersInput input)
		{
			try
			{
				var nelat = Math.Round(input.nelat.ToDouble(), Numbers.Round);
				var nelon = Math.Round(input.nelon.ToDouble(), Numbers.Round);
				var swlat = Math.Round(input.swlat.ToDouble(), Numbers.Round);
				var swlon = Math.Round(input.swlon.ToDouble(), Numbers.Round);
				var zoomLevel = int.Parse(input.zoomLevel);
				var filter = input.filter ?? "";

				// values are validated there
				var inputValidated = new JsonGetMarkersReceive(nelat, nelon, swlat, swlon, zoomLevel, filter);

				var grid = GridCluster.GetBoundaryExtended(inputValidated);
				var cacheKeyHelper = string.Format("{0}_{1}_{2}", inputValidated.Zoomlevel, inputValidated.FilterHashCode(), grid.GetHashCode());
				var cacheKey = CacheKeys.GetMarkers(cacheKeyHelper.GetHashCode());

				var reply = _memCache.Get<JsonMarkersReply>(cacheKey);
				if (reply != null)
				{
					// return cached data
					reply.Cache = true;
					return reply; 
				}

				inputValidated.Viewport.ValidateLatLon(); // Validate google map viewport input (should be always valid)
				inputValidated.Viewport.Normalize();

				// Get all points from memory
				ThreadData threadData = _pointsDatabase.GetThreadData();

				#region fiter

				// Filter points
				threadData = FilterUtil.FilterByType(
					threadData,
					new FilterData { TypeFilterExclude = inputValidated.TypeFilterExclude }
					);

				#endregion filter


				// Create new instance for every ajax request with input all points and json data
				ICluster clusterAlgo = new GridCluster(threadData, inputValidated);

				var clusteringEnabled = inputValidated.IsClusteringEnabled
					|| GmcSettings.Get.AlwaysClusteringEnabledWhenZoomLevelLess > inputValidated.Zoomlevel;

				// Clustering
				if (clusteringEnabled && inputValidated.Zoomlevel < GmcSettings.Get.ZoomlevelClusterStop)
				{
					#region cluster

					IList<P> markers = clusterAlgo.RunCluster();

					#endregion cluster

					reply = new JsonMarkersReply
					{
						Markers = markers,
						Polylines = clusterAlgo.GetPolyLines(),
					};
				}
				else
				{
					// If we are here then there are no clustering
					// The number of items returned is restricted to avoid json data overflow
					IList<P> filteredDataset = FilterUtil.FilterDataByViewport(threadData, inputValidated.Viewport).AllPoints;
					IList<P> filteredDatasetMaxPoints = filteredDataset.Take(GmcSettings.Get.MaxMarkersReturned).ToList();

					reply = new JsonMarkersReply
					{
						Markers = filteredDatasetMaxPoints,
						Polylines = clusterAlgo.GetPolyLines(),
						Mia = filteredDataset.Count - filteredDatasetMaxPoints.Count,
					};
				}
				
				// if client ne and sw is inside a specific grid box then cache the grid box and the result
				// next time test if ne and sw is inside the grid box and return the cached result				
				if(GmcSettings.Get.CacheServices) _memCache.Set(reply, cacheKey, TimeSpan.FromMinutes(10)); // cache data

				return reply;
			}
			catch (Exception ex)
			{
				return new JsonMarkersReply
				{
					Ok = "0",
					EMsg = string.Format("MapService says: exception {0}", ex.Message)
				};
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

		/// <summary>
		/// Read Through Cache
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public JsonMarkerInfoReply GetMarkerInfoHelper(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return new JsonMarkerInfoReply { Ok = "0", EMsg = "MapService says: params is invalid" };
			}
			try
			{
				var uid = int.Parse(id);

				var cacheKey = CacheKeys.GetMarkerInfo(uid);
				var reply = _memCache.Get<JsonMarkerInfoReply>(cacheKey);
				if (reply != null)
				{
					// return cached data
					reply.Cache = true;
					return reply;
				}

				P marker = _pointsDatabase.GetThreadData().AllPoints.SingleOrDefault(i => i.I == uid);

				reply = new JsonMarkerInfoReply { Id = id };
				reply.BuildContent(marker);

				if (GmcSettings.Get.CacheServices) _memCache.Set(reply, cacheKey, TimeSpan.FromMinutes(10)); // cache data

				return reply;
			}
			catch (Exception ex)
			{
				return new JsonMarkerInfoReply
				{
					Ok = "0",
					EMsg = string.Format("MapService says: Parsing error param: {0}", ex.Message)
				};
			}
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
				DbSize = _pointsDatabase.GetThreadData().AllPoints.Count,
				FirstPoint = _pointsDatabase.GetThreadData().AllPoints.FirstOrDefault()
			};

		}



		private T ReadThroughCache<T>(string key, Func<T> fn, TimeSpan cacheSpan)
			where T : class, new()
		{
			var data = _memCache.Get<T>(key);
			if (data == null)
			{
				data = fn.Invoke();
				if (data != null) _memCache.Set(data, key, cacheSpan);
			}
			return data;
		}
	}
}