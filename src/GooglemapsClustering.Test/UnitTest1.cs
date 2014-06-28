using System;
using System.Linq;
using Autofac;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
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
		static IMemoryDatabase memoryDatabase; // only loading points from file once, thus static
		IMapService mapService;

		[TestInitialize()]
		public void Initialize()
		{
			// Composition root, manual setup
			//memoryDatabase = memoryDatabase ?? new MemoryDatabase("TestData/Points.csv");
			//mapService = mapService ?? new MapService(memoryDatabase);

			// Composition root, using same setup as web
			bootstrap = bootstrap ?? new Bootstrap();
			bootstrap.Configure("TestData/Points.csv");
			memoryDatabase = memoryDatabase ?? bootstrap.Container.Resolve<IMemoryDatabase>();
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

		//[Ignore]
		[TestMethod]
		public void TestMethod1()
		{
			// Arrange

			// Act

			// Assert	
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
