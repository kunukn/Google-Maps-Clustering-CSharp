using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Kunukn.GooglemapsClustering.Clustering.Contract;

namespace Kunukn.GooglemapsClustering.Clustering.Data
{
    [Serializable]
    public class DatasetToSerialize : ISerializable
    {
        private const string Name = "Dataset";
        public IPoints Dataset { get; set; }
        public DatasetToSerialize()
        {
            Dataset = new Points();
        }
        public DatasetToSerialize(SerializationInfo info, StreamingContext ctxt)
        {
            this.Dataset = (IPoints)info.GetValue(Name, typeof(IPoints));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue(Name, this.Dataset);
        }
    }
}
