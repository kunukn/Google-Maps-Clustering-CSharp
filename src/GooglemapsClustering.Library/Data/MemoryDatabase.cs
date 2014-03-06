using System;
using System.Collections.Generic;
using System.Linq;

namespace GooglemapsClustering.Clustering.Data
{
    /// <summary>
    /// The database for all the existing points
    /// </summary>
    public static class MemoryDatabase
    {
        private static bool _isLoaded = false;     
        private static List<P> Points { get; set; }                
        private static string FilePath { get; set; }
        
        static MemoryDatabase()
        {
            Points = new List<P>();
        }

        public static List<P> GetPoints()
        {
            if (!_isLoaded)
            {
                Init();
            }
            return Points;            
        }


        public static void SetFilepath(string path)
        {
            // Only set once
            if (FilePath==null) FilePath = path;
        }

        
        public static void Init()
        {
            if (_isLoaded) return;

            // Load from file
            List<P> points = Utility.Dataset.LoadDataset(FilePath);            
            if (!points.Any() )
            {
                throw new Exception(string.Format("Data was not loaded from file: {0}",FilePath));
            }
            _isLoaded = true;

            if (points.Count > AlgoConfig.Get.MaxPointsInCache)
            {
                points = points.Take(AlgoConfig.Get.MaxPointsInCache).ToList();
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
        }   
    }
}
