using System;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Clustering.Data.Geometry
{
    [Serializable]
    public abstract class PBase
    {
        private static int _counter;        
        protected PBase(){ Uid = ++_counter; Init();}
                     
        public virtual int Uid { get; private set; }        
        public virtual object Data { get; set; } // Data container for anything
        
        public virtual double X { get { return Lon; } set { Lon = value; } }        
        public virtual double Y { get { return Lat; } set { Lat = value; } }
        public virtual int Type { get { return T; } set { T = value; } }
        
        public virtual int C { get; set; } // count
        public virtual int I { get; set; } // marker id           
        public virtual int T { get; set; } // marker type        
        public virtual string Name { get; set; } // custom

        private double _lat;
        public virtual double Lat
        {
            get { return _lat.Round(); }
            set { _lat = value; }
        }
        private double _lon;
        public virtual double Lon
        {
            get { return _lon.Round(); }
            set { _lon = value; }
        }

        void Init() { C = 1; }

        public virtual double Distance(PBase p)
        {
            return Distance(p.X, p.Y);
        }

        // Euclidean distance
        public virtual double Distance(double x, double y)
        {
            var dx = X - x;
            var dy = Y - y;
            var dist = (dx * dx) + (dy * dy);
            //dist = Math.Sqrt(dist);
            return dist;
        }

        public override bool Equals(object obj)
        {
            var o = obj as PBase;
            if (o == null) { return false; }
            return GetHashCode() == o.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }      
    }
}
