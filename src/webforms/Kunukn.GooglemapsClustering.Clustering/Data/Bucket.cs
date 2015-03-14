using System.Collections.Generic;
using Kunukn.GooglemapsClustering.Clustering.Contract;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    public class Bucket
    {
        public string Id { get; private set; }
        public IPoints Points { get; private set; }
        public IP Centroid { get; set; }
        public int Idx { get; private set; }
        public int Idy { get; private set; }
        public double ErrorLevel { get; set; } // clusterpoint and points avg dist
        private bool _isUsed;
        public bool IsUsed
        {
            get { return _isUsed && Centroid != null; }
            set { _isUsed = value; }
        }
        public Bucket(string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new Points();
            Id = id;
        }
        public Bucket(int idx, int idy, string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new Points();
            Idx = idx;
            Idy = idy;
            Id = id;
        }
    }
}
