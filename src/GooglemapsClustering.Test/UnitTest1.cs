using System;
using System.Linq;
using Autofac;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Data.Config;
using GooglemapsClustering.Clustering.Service;
using GooglemapsClustering.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GooglemapsClustering.Test
{
	/// <summary>
	/// todo make unit tests
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		static Bootstrap bootstrap;
		static IPointsDatabase memoryDatabase; // only loading points from file once, thus static
		IMapService mapService;

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


		[Ignore]
		[TestMethod]
		public void TestDatabaseThreadPointsCount()
		{
			// Arrange
			var sumThreadPoints = memoryDatabase.GetThreadPoints().Sum(list => list.Count);
			var sumPoints = memoryDatabase.GetPoints().Count;

			// Act

			// Assert	
			Assert.AreEqual(sumThreadPoints, sumPoints);
		}

		[Ignore]
		[TestMethod]
		public void TestMethod1()
		{
			// Arrange

			// Act

			// Assert	
			Assert.IsTrue(AlgoConfig.Get.Threads > 0);
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
