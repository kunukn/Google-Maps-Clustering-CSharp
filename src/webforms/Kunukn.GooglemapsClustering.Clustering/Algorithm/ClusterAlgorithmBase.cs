using System;
using System.Collections.Generic;
using System.Linq;
using Kunukn.GooglemapsClustering.Clustering.Contract;
using Kunukn.GooglemapsClustering.Clustering.Data;
using Kunukn.GooglemapsClustering.Clustering.Utility;

namespace Kunukn.GooglemapsClustering.Clustering.Algorithm
{
    /// <summary>
    /// /// Author: Kunuk Nykjaer
    /// </summary>
    public abstract class ClusterAlgorithmBase
    {        
        protected readonly IPoints Dataset; // all points
        //id, bucket
        public readonly Dictionary<string, Bucket> BucketsLookup =
            new Dictionary<string, Bucket>();

        protected ClusterAlgorithmBase() { }
        protected ClusterAlgorithmBase(IPoints dataset)
        {
            if (dataset == null)
            {
                throw new ApplicationException(string.Format("dataset is null"));
            }                
            Dataset = dataset;
        }

        public abstract IPoints GetCluster(ClusterInfo clusterInfo);

        public IPoints GetClusterResult(Boundary grid)
        {
            // Collect used buckets and return the result
            var clusterPoints = new Points();

            //O(m*n)
            foreach (var item in BucketsLookup)
            {
                var bucket = item.Value;
                if (!bucket.IsUsed) continue;

                if (bucket.Points.Count < AlgoConfig.MinClusterSize) clusterPoints.Data.AddRange(bucket.Points.Data);
                else
                {
                    bucket.Centroid.C = bucket.Points.Count;
                    clusterPoints.Add(bucket.Centroid);
                }
            }

            //var filtered = FilterDataset(clusterPoints, grid); // post filter data for client viewport
            //return filtered; //not working properly when zoomed far out.
            return clusterPoints;  // return not post filtered
        }

        // O(n), could be O(logn-ish) using range search or similar, no problem when points are <500.000
        public static IPoints FilterDataset(IPoints dataset, Boundary viewport)
        {
            //return new Points { Data = dataset.Data.Where(i => MathTool.IsInsideWiden(viewport, i)).ToList() };            
            return new Points {Data = dataset.Data.Where(i => MathTool.IsInside(viewport, i)).ToList()};
        }
        
        // Circular mean, very relevant for points around New Zealand, where lon -180 to 180 overlap
        // Adapted Centroid Calculation of N Points for Google Maps usage
        public static IP GetCentroidFromClusterLatLon(IPoints list) //O(n)
        {
            int count;
            if (list == null || (count = list.Count) == 0) return null;
                          
            if (count == 1)
            {
                return list.Data.First();
            }                

            // http://en.wikipedia.org/wiki/Circular_mean
            // http://stackoverflow.com/questions/491738/how-do-you-calculate-the-average-of-a-set-of-angles
            /*
                                  1/N*  sum_i_from_1_to_N sin(a[i])
                a = atan2      ---------------------------
                                  1/N*  sum_i_from_1_to_N cos(a[i])
             */

            double lonSin = 0;
            double lonCos = 0;
            double latSin = 0;
            double latCos = 0;
            foreach (var p in list.Data)
            {
                lonSin += Math.Sin(p.X.LatLonToRadian());
                lonCos += Math.Cos(p.X.LatLonToRadian());
                latSin += Math.Sin(p.Y.LatLonToRadian());
                latCos += Math.Cos(p.Y.LatLonToRadian());
            }

            lonSin /= count;
            lonCos /= count;

            double radx = 0;
            double rady = 0;

            if (Math.Abs(lonSin - 0) > Numbers.Epsilon && Math.Abs(lonCos - 0) > Numbers.Epsilon)
            {
                radx = Math.Atan2(lonSin, lonCos);
                rady = Math.Atan2(latSin, latCos);
            }
            var x = radx.RadianToLatLon();
            var y = rady.RadianToLatLon();

            var centroid = new P{X=x, Y=y, C = count };
            return centroid;
        }


        // O(k*n)
        public static void SetCentroidForAllBuckets(IEnumerable<Bucket> buckets)
        {
            foreach (var item in buckets)
            {
                item.Centroid = GetCentroidFromClusterLatLon(item.Points);
            }                
        }

        public IP GetClosestPoint(IP from, IPoints list) // O(n)
        {
            var min = double.MaxValue;
            IP closests = null;
            foreach (var p in list.Data)
            {
                var d = MathTool.Distance(from, p);
                if (d >= min)
                {
                    continue;
                }
                    
                // update
                min = d;
                closests = p;
            }
            return closests;
        }

        // Assign all points to nearest cluster
        public void UpdatePointsByCentroid() // O(n*k)
        {
            // Clear points in the buckets, they will be re-inserted
            foreach (var bucket in BucketsLookup.Values)
            {
                bucket.Points.Data.Clear();
            }                

            foreach (var p in Dataset.Data)
            {
                var minDist = Double.MaxValue;
                var index = string.Empty;
                foreach (var i in BucketsLookup.Keys)
                {
                    var bucket = BucketsLookup[i];
                    if (bucket.IsUsed == false)
                    {
                        continue;
                    }
                        
                    var centroid = bucket.Centroid;
                    var dist = MathTool.Distance(p, centroid);
                    if (dist < minDist)
                    {
                        // update
                        minDist = dist;
                        index = i;
                    }
                }
                // re-insert
                var closestBucket = BucketsLookup[index];
                closestBucket.Points.Add(p);
            }
        }

        // Update centroid location to nearest point, 
        // e.g. if you want to show cluster point on a real existing point area
        // O(n)
        public void UpdateCentroidToNearestContainingPoint(Bucket bucket)
        {
            if (bucket == null || bucket.Centroid == null ||
                bucket.Points == null || bucket.Points.Count == 0)
            {
                return;
            }
                
            var closest = GetClosestPoint(bucket.Centroid, bucket.Points);
            bucket.Centroid.X = closest.X; // no normalize, points are already normalized by default
            bucket.Centroid.Y = closest.Y;
        }
        // O(k*n)
        public void UpdateAllCentroidsToNearestContainingPoint()
        {
            foreach (var bucket in BucketsLookup.Values)
            {
                UpdateCentroidToNearestContainingPoint(bucket);
            }                
        }


        #region NOT USED
        /*
        // Can be used instead of GetCentroidFromClusterLatLon if your data are not near lon 180
        // Basic Centroid Calculation of N Points
        public static IP GetCentroidFromCluster(IPoints list) //O(n)
        {          
            int count;
            if (list == null || (count = list.Count) == 0)
                return null;

            if (list.Count == 1) return list.Data.First();

            var centroid = new P(0, 0) { C = count };//O(1)
            foreach (var p in list.Data)
            {
                centroid.Lon += p.Lon;
                centroid.Lat += p.Lat;
            }
            centroid.Lon /= count;
            centroid.Lat /= count;                        
            return centroid;
        }
        */
        #endregion NOT USED
    }
}
