using System.Threading.Tasks;
using Kunukn.GooglemapsClustering.Clustering.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Utility
{
    public static class GmcInit
    {
        private static bool _done;
        /// <summary>
        /// Init with file path to csv file
        /// </summary>
        /// <param name="path"></param>
        public static void Init(string path)
        {
            // Only run once
            if (_done) return;
            _done = true;

            // Load in a new thread, faster UI display
            var task = new Task(() => Run(path));
            task.Start();
        }

        private static void Run(string path)
        {
            // Database load simulation            
            MemoryDatabase.SetFilepath(path);
            var points = MemoryDatabase.GetPoints(); // preload points into memory            
        }
    }
}
