using System;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IMemCache
	{
		T Get<T>(string key) where T : class;
		bool Add<T>(T objectToCache, string key, TimeSpan timespan);
		object Clear(string key);
		bool Exists(string key);		
	}
}
