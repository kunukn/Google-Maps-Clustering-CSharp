using System;
using System.Runtime.Serialization;
using Kunukn.GooglemapsClustering.Clustering.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Contract
{    
    public interface IP : ISerializable
    {
        double X { get; set; }  // lon
        double Y { get; set; }  // lat
        double Lon { get; set; }  // lon
        double Lat { get; set; }  // lat
        int C { get; set; }     // count of cluster
        int I { get; set; }  // marker id
        int T { get; set; }  // marker type

        object Data { get; set; } // data container for anything, use it as you like
        
        IP Normalize();
        double Distance(PBase p);
        double Distance(double x, double y);
    }
}
