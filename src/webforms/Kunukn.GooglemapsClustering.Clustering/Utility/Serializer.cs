using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kunukn.GooglemapsClustering.Clustering.Utility
{
    /// <summary>
    /// Author: Kunuk Nykjaer
    /// </summary>
    public class Serializer
    {
        public void SerializeObject(string filepath, object objectToSerialize)
        {
            try
            {
                FileUtil.CreateFilePath(filepath); // create folder if not exists
                using (Stream stream = File.Open(filepath, FileMode.Create))
                {
                    var bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(stream, objectToSerialize);
                }
            }
            catch (Exception ex)
            {
                throw ex;                
            }
        }
        public object DeSerializeObject(string filepath)
        {
            try
            {
                using (Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    var bFormatter = new BinaryFormatter();
                    var objectToSerialize = bFormatter.Deserialize(stream);
                    return objectToSerialize;
                }
            }
            catch (Exception ex)
            {
                throw ex;                
            }            
        }
    }
}
