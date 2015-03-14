using System;
using Kunukn.GooglemapsClustering.Clustering.Utility;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    public class Boundary : Rectangle
    {        
        public Boundary() { }
        public Boundary(Boundary b)
        {            
            this.Minx = b.Minx;
            this.Miny = b.Miny;
            this.Maxx = b.Maxx;
            this.Maxy = b.Maxy;
        }


        /// <summary>
        /// Normalize lat and lon values to their boundary values
        /// </summary>
        public void Normalize()
        {
            Minx = Minx.NormalizeLongitude();
            Maxx = Maxx.NormalizeLongitude();
            Miny = Miny.NormalizeLatitude();
            Maxy = Maxy.NormalizeLatitude();
        }


        public void ValidateLatLon()
        {
            if ((Minx > LatLonInfo.MaxLonValue || Minx < LatLonInfo.MinLonValue)
                || (Maxx > LatLonInfo.MaxLonValue || Maxx < LatLonInfo.MinLonValue)
                || (Miny > LatLonInfo.MaxLatValue || Miny < LatLonInfo.MinLatValue)
                || (Maxy > LatLonInfo.MaxLatValue || Maxy < LatLonInfo.MinLatValue)
                )
                throw new ApplicationException("input Boundary.ValidateLatLon() error " + this);
        }

        // Distance lon
        public double AbsX { get { return Minx.AbsLon(Maxx); } }

        // Distance lat
        public double AbsY { get { return Miny.AbsLat(Maxy); } }
    }
}
