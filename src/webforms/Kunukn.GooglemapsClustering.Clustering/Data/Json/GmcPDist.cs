using System;
using System.Runtime.Serialization;
using Kunukn.SingleDetectLibrary.Code.Contract;
using Kunukn.SingleDetectLibrary.Code.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Data.Json
{
    [Serializable]
    public class GmcPDist : PDist, IPDist
    {
        public string Id { get; set; }

        public GmcPDist()
        {
            Id = "";
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", this.Point.Uid);
            info.AddValue("X", this.Point.X);
            info.AddValue("Y", this.Point.Y);
            info.AddValue("Type", this.Point.Type);
            info.AddValue("Dist", this.Distance);
        }

        public override string ToString()
        {
            var s = base.ToString();
            return string.Format("Id: {0}, {1}", Id, s);
        }
    }
}
