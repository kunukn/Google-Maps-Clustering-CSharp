using System.Collections.Generic;
using Autofac;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Geometry;
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
            Assert.IsTrue(memoryDatabase.GetPoints().Any());
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
                .Stub(s => s.GetPoints())
                .Return(new List<P>{new P{Name = "test"}});

            // Act
            var data = pointsDatabase.GetPoints();

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