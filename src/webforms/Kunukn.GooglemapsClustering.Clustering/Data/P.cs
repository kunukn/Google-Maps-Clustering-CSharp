using System;
using System.Runtime.Serialization;
using Kunukn.GooglemapsClustering.Clustering.Contract;
using Kunukn.GooglemapsClustering.Clustering.Utility;
using Kunukn.SingleDetectLibrary.Code.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    /// <summary>
    /// Point class, overwrite it, modify it, extend it as you like
    /// </summary>
    [Serializable]
    public class P : PBase,                         // Gmc 
        IP, SingleDetectLibrary.Code.Contract.IP,   // K-nearest neighbors        
        ISerializable                               // Json data
    {
        public P()
        {
            GridIndex = new GridIndex();
        }
        public P(double x, double y) : this()
        {
            X = x;
            Y = y;
        }


        public virtual IP Normalize()
        {
            Lon = Lon.NormalizeLongitude();
            Lat = Lat.NormalizeLatitude();
            return this;
        }

        // Dist betwee two points on Earth
        public new virtual double Distance(double x, double y)
        {
            return MathTool.Haversine(this.Y, this.X, y, x);
        }

        public override string ToString()
        {
            return string.Format("Uid: {0}, X:{1}, Y:{2}, T:{3}, I:{4}",
                Uid, X, Y, T, I);
        }

        public virtual GridIndex GridIndex { get; set; }


        // Used for e.g. serialization to file
        public P(SerializationInfo info, StreamingContext ctxt)
        {
            this.C = 1;
            this.I = (int)info.GetValue("I", typeof(int));
            this.T = (int)info.GetValue("T", typeof(int));
            this.X = ((string)info.GetValue("X", typeof(string))).ToDouble();
            this.Y = ((string)info.GetValue("Y", typeof(string))).ToDouble();
        }

        // Data returned as Json
        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("I", this.I);
            info.AddValue("T", this.T);
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("C", this.C);
        }

        public int CompareTo(P other, int dimension)
        {
            if (dimension == 0)
                return this.X.CompareTo(other.X);
            if (dimension == 1)
                return this.Y.CompareTo(other.Y);
            throw new ArgumentException("Invalid dimension.");
        }
    }
}
