using System;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IMemCache
	{
		T Get<T>(string key) where T : class;
		bool Add<T>(T objectToCache, string key, TimeSpan timespan);
	    void Set<T>(T objectToCache, string key, TimeSpan timespan);
		object Remove(string key);
		bool Exists(string key);		
	}
}
