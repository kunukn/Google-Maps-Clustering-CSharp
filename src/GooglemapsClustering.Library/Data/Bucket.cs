using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Data
{
    public class Bucket
    {
        public string Id { get; private set; }
        public List<P> Points { get; private set; }
        public P Centroid { get; set; }
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
            Points = new List<P>();
            Id = id;
        }
        public Bucket(int idx, int idy, string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new List<P>();
            Idx = idx;
            Idy = idy;
            Id = id;
        }
    }
}
