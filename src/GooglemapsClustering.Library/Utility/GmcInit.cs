using GooglemapsClustering.Clustering.Data;

namespace GooglemapsClustering.Clustering.Utility
{
    public static class GmcInit
    {        
        /// <summary>
        /// Init with file path to csv file
        /// </summary>
        /// <param name="path"></param>
        public static void Init(string path)
        {
            // Database load simulation            
            MemoryDatabase.SetFilepath(path);
            MemoryDatabase.Init(); // preload points into memory            
        }        
    }
}
