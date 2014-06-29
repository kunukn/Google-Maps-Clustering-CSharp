using System;

namespace GooglemapsClustering.Clustering.Data.Geometry
{
    [Serializable]
    public class Rectangle
    {
        public double Minx { get; set; }
        public double Maxx { get; set; }
        public double Miny { get; set; }
        public double Maxy { get; set; }

        public override string ToString()
        {
            return string.Format("minx: {0} miny: {1} maxx: {2} maxy: {3}", 
                Minx, Miny, Maxx, Maxy);
        }
    }
}
