using System;
using System.Runtime.Serialization;

namespace GooglemapsClustering.Clustering.Data.Geometry
{
    /// <summary>
    /// Used for polylines display on google maps
    /// </summary>
    [Serializable]    
    public class Line : ISerializable
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string X2 { get; set; }
        public string Y2 { get; set; }

        public Line()
        {
            X = string.Empty;
            Y = string.Empty;
            X2 = string.Empty;
            Y2 = string.Empty;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {            
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("X2", this.X2);
            info.AddValue("Y2", this.Y2);            
        }
    }
}
