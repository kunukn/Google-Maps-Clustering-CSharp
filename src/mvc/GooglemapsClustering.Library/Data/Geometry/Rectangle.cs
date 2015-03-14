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

	    public override int GetHashCode()
	    {
			return string.Format("{0}_{1}_{2}_{3}", Minx, Maxx, Miny, Maxy).GetHashCode();
	    }
    }
}
