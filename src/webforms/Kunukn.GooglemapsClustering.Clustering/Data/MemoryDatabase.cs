using System;
using System.Collections.Generic;
using System.Linq;

using Kunukn.SingleDetectLibrary.Code;
using Kunukn.SingleDetectLibrary.Code.Contract;
using P = Kunukn.GooglemapsClustering.Clustering.Data.P;
using IP = Kunukn.GooglemapsClustering.Clustering.Contract.IP;
using IPoints = Kunukn.GooglemapsClustering.Clustering.Contract.IPoints;

// K Nearest Neighbor
using IKnnAlgorithm = Kunukn.SingleDetectLibrary.Code.Contract.IAlgorithm;
using KnnAlgorithm = Kunukn.SingleDetectLibrary.Code.Algorithm;
using IPsKnn = Kunukn.SingleDetectLibrary.Code.Contract.IPoints;
using IPKnn = Kunukn.SingleDetectLibrary.Code.Contract.IP;
using IPointsKnn = Kunukn.SingleDetectLibrary.Code.Contract.IPoints;
using PointsKnn = Kunukn.SingleDetectLibrary.Code.Data.Points;
using PKnn = Kunukn.SingleDetectLibrary.Code.Data.P;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    /// <summary>
    /// The database for all the existing points
    /// </summary>
    public static class MemoryDatabase
    {        
        private static bool _flag;
        private static IPoints Points { get; set; }
        private static IPoints PointsBackup { get; set; }
        public static object Data { get; private set; } // data container
        private static string FilePath { get; set; }
        private static readonly Object Lock = new Object();
        

        static MemoryDatabase()
        {
            Points = new Points();
            PointsBackup = new Points();
        }

        public static IPoints GetPoints()
        {
            if (Points != null && Points.Count > 0) return Points;          
            return LoadPoints();
        }
              

        public static void SetFilepath(string path)
        {
            // Only set once
            if (FilePath==null) FilePath = path;
        }

        private static IPoints LoadPoints()
        {
            lock (Lock)
            {
                if (_flag)
                {
                    // Alternative is to throw exception
                    // If you are here, then points were deleted accidently from code somewhere
                    Points.SetRange(PointsBackup);
                    return Points;
                }
                _flag = true;
            }


            // Load from file

            var points = Utility.Dataset.LoadDataset(FilePath);            
            if (points == null || !points.Data.Any() )
            {
                throw new ApplicationException("Data was not loaded from file");
            }
                        
            // Randomize order, when limit take is used for max marker display
            // random locations are selected
            var rand = new Random();
            var c = points.Count;
            for (var i = 0; i < c; i++)
            {
                //var p = points[i]; // do something with each p ?        
                var a = rand.Next(c);
                var b = rand.Next(c);
                var temp = points[a];
                points[a] = points[b];
                points[b] = temp;
            }

            Points = points;
            PointsBackup.SetRange(points);
            SetKnnAlgo(points); // K Nearest neighbor algorithm            

            return Points;
        }
  

        // Read about this at github.com/kunukn/single-detect
        static void SetKnnAlgo(IPoints points)
        {            
            IPointsKnn dataset = new PointsKnn();
            dataset.Data.AddRange(points.Data.Select(i => i as IPKnn));            
            var rect = new SingleDetectLibrary.Code.Data.Rectangle
            {
                XMin = -190,
                XMax = 190,
                YMin = -100,
                YMax = 100,
                MaxDistance = 20,
            };
            rect.Validate();
            
            // Naive stratey works with all points on Earth.
            // Grid strategy runs much faster and can be used as approx algo 
            // but only works on certain local areas, not wrapped world. e.g. from lon -90 to lon 90, 
            // e.g. Europe only areas or US only areas etc. but not New Zealand due to being near lon 180.
            // All points must be within rect boundary
            IKnnAlgorithm algo = new KnnAlgorithm(dataset, rect, StrategyType.Naive);          
            Data = algo;
        }       
    }
}
