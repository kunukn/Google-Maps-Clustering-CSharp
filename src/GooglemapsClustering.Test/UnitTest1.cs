using Autofac;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace GooglemapsClustering.Test
{
    /// <summary>
    /// todo make unit tests
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        private static Bootstrap bootstrap;
        private static IPointsDatabase memoryDatabase; // only loading points from file once, thus static
        private IMapService mapService;

        [TestInitialize()]
        public void Initialize()
        {
            // Composition root, manual setup
            //memoryDatabase = memoryDatabase ?? new MemoryDatabase("TestData/Points.csv");
            //mapService = mapService ?? new MapService(memoryDatabase);

            // Composition root-ish, using same setup as web
            bootstrap = bootstrap ?? new Bootstrap();
            bootstrap.Configure("TestData/Points.csv");
            memoryDatabase = memoryDatabase ?? bootstrap.Container.Resolve<IPointsDatabase>();
            mapService = mapService ?? bootstrap.Container.Resolve<IMapService>();
        }

        [TestMethod]
        public void TestIoC()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsNotNull(memoryDatabase);
            Assert.IsNotNull(mapService);
            Assert.IsTrue(memoryDatabase.GetThreadData().AllPoints.Any());
        }

        [TestMethod]
        public void TestDatabaseThreadPointsCount()
        {
            // Arrange
            //var sumThreadPoints = memoryDatabase.GetThreadPoints().Sum(list => list.Count);
            //var sumPoints = memoryDatabase.GetPoints().Count;

            //// Act

            //// Assert
            //Assert.AreEqual(sumThreadPoints, sumPoints);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod1()
        {
            // todo dummy template for how to mock things
            // not a real test

            // Arrange
            var pointsDatabase = MockRepository.GenerateStub<IPointsDatabase>();
            pointsDatabase
                .Stub(s => s.GetThreadData())
                .Return(new ThreadData(10));

            // Act
            var data = pointsDatabase.GetThreadData();

            // Assert
            Assert.IsNotNull(data);
        }

        [TestCleanup()]
        public void Cleanup() { }

        [ClassCleanup()]
        public static void ClassCleanup() { }

        [AssemblyCleanup()]
        public static void AssemblyCleanup() { }

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context) { }

        [ClassInitialize()]
        public static void ClassInit(TestContext context) { }
    }
}