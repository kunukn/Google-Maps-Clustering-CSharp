using GooglemapsClustering.Clustering.Data.Geometry;
using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Data
{
    public class ThreadData
    {
        public int Threads { get; private set; }

        // todo upd when threads support is fully implemented
        // todo currently multiple threads are not supported, this is working in progress implementation

        public IList<P> AllPoints
        {
            get { return ThreadPoints[0]; }
            set { ThreadPoints[0] = value; }
        }
        public IList<P>[] ThreadPoints { get; set; }

        public ThreadData(int threads)
        {
            if (threads < 1) threads = 1;
            if (threads > 100) threads = 100;

            Threads = threads;
            Init();
        }

        private void Init()
        {
            //AllPoints = new List<P>();
            ThreadPoints = new IList<P>[Threads];
            for (int i = 0; i < Threads; i++) ThreadPoints[i] = new List<P>();
        }

        public IList<P> this[int i]
        {
            get { return this.ThreadPoints[i]; }
            set { this.ThreadPoints[i] = value; }
        }
    }
}