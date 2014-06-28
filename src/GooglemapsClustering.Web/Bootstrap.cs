using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data;
using GooglemapsClustering.Clustering.Service;

namespace GooglemapsClustering.Web
{
	/// <summary>
	/// DI Container
	/// Autofac
	/// </summary>
	public class Bootstrap
	{
		public IContainer Container { get; private set; }

		public void Configure(string filePathToPoints)
		{
			var builder = new ContainerBuilder();
			OnConfigure(builder, filePathToPoints);

			if (this.Container == null) this.Container = builder.Build();
			else builder.Update(this.Container);

			DependencyResolver.SetResolver(new AutofacDependencyResolver(this.Container));
		}

		protected virtual void OnConfigure(ContainerBuilder builder, string filePathToPoints)
		{
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			// Important it is singleton!, loading the points from file can be slow and expensive
			builder.Register(c => new MemoryDatabase(filePathToPoints, AlgoConfig.Get.Threads)).As<IMemoryDatabase>().SingleInstance(); 
			
			builder.RegisterType<MapService>().As<IMapService>();
		}
	}
}