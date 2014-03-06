using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GooglemapsClustering.Clustering.Data
{
    [Serializable]
    public class DatasetToSerialize : ISerializable
    {
        private const string Name = "Dataset";
        public List<P> Dataset { get; set; }
        public DatasetToSerialize()
        {
            Dataset = new List<P>();
        }
        public DatasetToSerialize(SerializationInfo info, StreamingContext ctxt)
        {
            this.Dataset = (List<P>)info.GetValue(Name, typeof(List<P>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue(Name, this.Dataset);
        }
    }
}
