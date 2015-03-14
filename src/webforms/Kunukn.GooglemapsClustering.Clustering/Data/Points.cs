using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Kunukn.GooglemapsClustering.Clustering.Contract;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    public class Points : IPoints, ISerializable
    {
        public int Count { get { return Data.Count; } }
        public List<IP> Data { get; set; }

        public Points()
        {
            Data = new List<IP>();            
        }

        public IP this[int i]
        {
            get { return Data[i]; }
            set { Data[i] = value; }
        }

        public void Add(IP p)
        {
            Data.Add(p);
        }

        public IList<IP> ToList()
        {
            return this.Data.ToList();
        }

        public void SetRange(IPoints points)
        {
            this.Data.Clear();
            this.Data.AddRange(points.Data);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var p in Data) sb.AppendFormat("[{0}] ", p);
            return sb.ToString();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Data", this.Data);           
        }
    }
}
