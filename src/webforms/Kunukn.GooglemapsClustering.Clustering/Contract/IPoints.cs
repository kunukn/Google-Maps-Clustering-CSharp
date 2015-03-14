using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kunukn.GooglemapsClustering.Clustering.Contract
{
    public interface IPoints : ISerializable
    {
        int Count { get ; } 
        List<IP> Data { get; set; } // uses AddRange
        IP this[int i] { get; set; }
        void Add(IP p);
        IList<IP> ToList();
        void SetRange(IPoints points);
    }
}
