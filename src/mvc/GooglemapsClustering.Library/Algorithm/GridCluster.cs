using System;
using System.Collections.Generic;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Algo;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Data.Json;
using GooglemapsClustering.Clustering.Extensions;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Clustering.Algorithm
{
	/// <summary>
	/// Author: Kunuk Nykjaer
	/// </summary>
	public class GridCluster : ClusterAlgorithmBase, ICluster
	{
		private readonly JsonGetMarkersReceive _jsonReceive;

		// Absolut position
		protected readonly Boundary Grid = new Boundary();

		// Bucket placement calc, grid cluster algo
		protected readonly double DeltaX;
		protected readonly double DeltaY;

		public static Boundary GetBoundaryExtended(JsonGetMarkersReceive input)
		{            
            var deltas = GetDelta(GmcSettings.Get.Gridx, GmcSettings.Get.Gridy, input.Zoomlevel);
			var deltaX = deltas[0];
			var deltaY = deltas[1];

			// Grid with extended outer grid-area non-visible            
			var a = MathTool.FloorLatLon(input.Viewport.Minx, deltaX) - deltaX * GmcSettings.Get.OuterGridExtend;
			var b = MathTool.FloorLatLon(input.Viewport.Miny, deltaY) - deltaY * GmcSettings.Get.OuterGridExtend;
			var a2 = MathTool.FloorLatLon(input.Viewport.Maxx, deltaX) + deltaX * (1 + GmcSettings.Get.OuterGridExtend);
			var b2 = MathTool.FloorLatLon(input.Viewport.Maxy, deltaY) + deltaY * (1 + GmcSettings.Get.OuterGridExtend);

			// Latitude is special with Google Maps, they don't wrap around, then do constrain
			b = MathTool.ConstrainLatitude(b);
			b2 = MathTool.ConstrainLatitude(b2);

			var grid = new Boundary { Minx = a, Miny = b, Maxx = a2, Maxy = b2 };
			grid.Normalize();
			return grid;
		}


		/// <summary>
		/// O(1)
		/// </summary>
		/// <returns></returns>
		public static double[] GetDelta(int gridx, int gridy, int zoomlevel)
		{
			// Heuristic specific values and grid size dependent.
			// used in combination with zoom level.

			// xZoomLevel1 and yZoomLevel1 is used to define the size of one grid-cell

			// Absolute base value of longitude distance, heuristic value
			const int xZoomLevel1 = 480;
			// Absolute base value of latitude distance, heuristic value
			const int yZoomLevel1 = 240;

			// Relative values, used for adjusting grid size			
			var x = MathTool.Half(xZoomLevel1, zoomlevel - 1) / gridx;
			var y = MathTool.Half(yZoomLevel1, zoomlevel - 1) / gridy;
			return new double[] { x, y };
		}



		/// <summary>
		/// todo use threads and threadData
		/// </summary>
		/// <param name="points"></param>
		/// <param name="input"></param>
		public GridCluster(IList<P> points, JsonGetMarkersReceive input)
			: base(points)
		{
			this._jsonReceive = input;
            double[] deltas = GetDelta(GmcSettings.Get.Gridx, GmcSettings.Get.Gridy, input.Zoomlevel);
			DeltaX = deltas[0];
			DeltaY = deltas[1];
			Grid = GetBoundaryExtended(input);
		}

		/// <summary>
		/// For debugging purpose
		/// Display the red grid lines on the map to show how the points are clustered
		/// </summary>
		/// <returns></returns>
		public IList<Line> GetPolyLines()
		{
			if (!GmcSettings.Get.DoShowGridLinesInGoogleMap) return null; // server disabled it
			if (!_jsonReceive.IsDebugLinesEnabled) return null; // client disabled it

			// Make the red lines data to be drawn in Google map

			var temp = new List<Rectangle>();

			const int borderLinesAdding = 1;
			var linesStepsX = (int)(Math.Round(Grid.AbsX / DeltaX) + borderLinesAdding);
			var linesStepsY = (int)(Math.Round(Grid.AbsY / DeltaY) + borderLinesAdding);

			var b = new Boundary(Grid);
			const double restrictLat = 5.5;	 // heuristic value, Google maps related
			b.Miny = MathTool.ConstrainLatitude(b.Miny, restrictLat); // Make sure it is visible on screen, restrict by some value
			b.Maxy = MathTool.ConstrainLatitude(b.Maxy, restrictLat);

			// Vertical lines
			for (var i = 0; i < linesStepsX; i++)
			{
				var xx = b.Minx + i * DeltaX;

				// Draw region
				if (_jsonReceive.Zoomlevel > 3)	// heuristic value, Google maps related
				{
					temp.Add(new Rectangle { Minx = xx, Miny = b.Miny, Maxx = xx, Maxy = b.Maxy });
				}
				// World wrap issue when same latlon area visible multiple times
				// Make sure line is drawn from left to right on screen
				else
				{
					temp.Add(new Rectangle { Minx = xx, Miny = LatLonInfo.MinLatValue + restrictLat, Maxx = xx, Maxy = 0 });
					temp.Add(new Rectangle { Minx = xx, Miny = 0, Maxx = xx, Maxy = LatLonInfo.MaxLatValue - restrictLat });
				}
			}

			// Horizontal lines            
			for (var i = 0; i < linesStepsY; i++)
			{
				var yy = b.Miny + i * DeltaY;

				// Draw region
				if (_jsonReceive.Zoomlevel > 3)  // heuristic value
				{
					// Don't draw lines outsize the world
					if (MathTool.IsLowerThanLatMin(yy) || MathTool.IsGreaterThanLatMax(yy)) continue;

					temp.Add(new Rectangle { Minx = b.Minx, Miny = yy, Maxx = b.Maxx, Maxy = yy });
				}
				// World wrap issue when same latlon area visible multiple times
				// Make sure line is drawn from left to right on screen
				else
				{
					temp.Add(new Rectangle { Minx = LatLonInfo.MinLonValue, Miny = yy, Maxx = 0, Maxy = yy });
					temp.Add(new Rectangle { Minx = 0, Miny = yy, Maxx = LatLonInfo.MaxLonValue, Maxy = yy });
				}
			}

			var lines = new List<Line>();

			// Normalize the lines and add as string
			foreach (var line in temp)
			{
				var x = (line.Minx).NormalizeLongitude().DoubleToString();
				var x2 = (line.Maxx).NormalizeLongitude().DoubleToString();
				var y = (line.Miny).NormalizeLatitude().DoubleToString();
				var y2 = (line.Maxy).NormalizeLatitude().DoubleToString();
				lines.Add(new Line { X = x, Y = y, X2 = x2, Y2 = y2 });
			}
			return lines;
		}		

		// Dictionary lookup key used by grid cluster algo
		public static string GetId(int idx, int idy) //O(1)
		{
			return string.Concat(idx, ";", idy);
		}

		// Average running time (m*n)
		// worst case might actually be 
		// ~ O(n^2) if most of centroids are merged, due to centroid re-calculation, very very unlikely
		void MergeClustersGrid()
		{
			foreach (var key in BucketsLookup.Keys)
			{
				var bucket = BucketsLookup[key];
				if (!bucket.IsUsed) continue; // skip not used

				var x = bucket.Idx;
				var y = bucket.Idy;

				// get keys for neighbors
				var N = GetId(x, y + 1);
				var NE = GetId(x + 1, y + 1);
				var E = GetId(x + 1, y);
				var SE = GetId(x + 1, y - 1);
				var S = GetId(x, y - 1);
				var SW = GetId(x - 1, y - 1);
				var W = GetId(x - 1, y);
				var NW = GetId(x - 1, y - 1);
				var neighbors = new[] { N, NE, E, SE, S, SW, W, NW };

				MergeClustersGridHelper(key, neighbors);
			}
		}
		void MergeClustersGridHelper(string currentKey, IEnumerable<string> neighborKeys)
		{
			double minDistX = DeltaX / GmcSettings.Get.MergeWithin;
			double minDistY = DeltaY / GmcSettings.Get.MergeWithin;
			// If clusters in grid are too close to each other, merge them
			double withinDist = Math.Max(minDistX, minDistY);

			foreach (var neighborKey in neighborKeys)
			{
				if (!BucketsLookup.ContainsKey(neighborKey)) continue;

				var neighbor = BucketsLookup[neighborKey];
				if (neighbor.IsUsed == false) continue;

				var current = BucketsLookup[currentKey];
				var dist = MathTool.Distance(current.Centroid, neighbor.Centroid);
				if (dist > withinDist) continue;

				current.Points.AddRange(neighbor.Points); //O(n)

				// recalc centroid
				var centroidPoint = GetCentroidFromClusterLatLon(current.Points);
				current.Centroid = centroidPoint;
				neighbor.IsUsed = false; // merged, then not used anymore
				neighbor.Points.Clear(); // clear mem
			}
		}

		// To work properly it requires the p is already normalized
		protected static int[] GetPointMappedIds(P p, Boundary grid, double deltax, double deltay)
		{
			#region Naive
			// Naive version, lon points near 180 and lat points near 90 are not clustered together
			//idx = (int)(relativeX / deltax);
			//idy = (int)(relativeY / deltay);
			//var relativeX = p.X - grid.Minx;
			#endregion Naive

			/*
			You have to draw a line with longitude values 180, -180 on papir to understand this            
                
			 e.g. _deltaX = 20
longitude        150   170  180  -170   -150
				 |      |          |     |
                 
       
   idx =         7      8    9    -9    -8
							-10    
                                  
here we want idx 8, 9, -10 and -9 be equal to each other, we set them to idx=8
then the longitudes from 170 to -170 will be clustered together
			 */

			var relativeY = p.Y - grid.Miny;

			var overlapMapMinX = (int)(LatLonInfo.MinLonValue / deltax) - 1;
			var overlapMapMaxX = (int)(LatLonInfo.MaxLonValue / deltax);

			// The deltaX = 20 example scenario, then set the value 9 to 8 and -10 to -9            

			// Similar to if (LatLonInfo.MaxLonValue % deltax == 0) without floating presicion issue
			if (Math.Abs(LatLonInfo.MaxLonValue % deltax - 0) < Numbers.Epsilon)
			{
				overlapMapMaxX--;
				overlapMapMinX++;
			}

			var idxx = (int)(p.X / deltax);
			if (p.X < 0) idxx--;

			if (Math.Abs(LatLonInfo.MaxLonValue % p.X - 0) < Numbers.Epsilon)
			{
				if (p.X < 0) idxx++;
				else idxx--;
			}
			if (idxx == overlapMapMinX) idxx = overlapMapMaxX;

			var idx = idxx;

			// Latitude never wraps around with Google Maps, ignore 90, -90 wrap-around for latitude
			var idy = (int)(relativeY / deltay);

			return new[] { idx, idy };
		}


		public IList<P> RunCluster()
		{
			// Skip points outside the grid, not visible to user then skip those
			IList<P> filtered = ClusterInfo.DoFilterData(this._jsonReceive.Zoomlevel)
				? FilterUtil.FilterDataByViewport(this.points, Grid)
				: this.points;

			// Put points in buckets
			foreach (var p in filtered)
			{
				var idxy = GetPointMappedIds(p, Grid, DeltaX, DeltaY);
				var idx = idxy[0];
				var idy = idxy[1];

				// Bucket id
				var id = GetId(idx, idy);

				// Bucket exists, add point
				if (BucketsLookup.ContainsKey(id))
				{
					BucketsLookup[id].Points.Add(p);
				}
				// New bucket, create and add point
				else
				{
					var bucket = new Bucket(idx, idy, id);
					bucket.Points.Add(p);
					BucketsLookup.Add(id, bucket);
				}
			}

			// Calculate centroid for all buckets
			SetCentroidForAllBuckets(BucketsLookup.Values);

			// Merge if gridpoint is to close
			if (GmcSettings.Get.DoMergeGridIfCentroidsAreCloseToEachOther) MergeClustersGrid();

			if (GmcSettings.Get.DoUpdateAllCentroidsToNearestContainingPoint) UpdateAllCentroidsToNearestContainingPoint();

			// Check again
			// Merge if gridpoint is to close
			if (GmcSettings.Get.DoMergeGridIfCentroidsAreCloseToEachOther
				&& GmcSettings.Get.DoUpdateAllCentroidsToNearestContainingPoint)
			{
				MergeClustersGrid();
				// And again set centroid to closest point in bucket 
				UpdateAllCentroidsToNearestContainingPoint();
			}

			return GetClusterResult(Grid);
		}
	}
}