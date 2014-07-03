using System;
using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Contract
{
	public interface IMemCache
	{
		T Get<T>(string key) where T : class;
		bool Add(object objectToCache, string key, TimeSpan timespan);
		object Clear(string key);
		bool Exists(string key);
		IList<string> GetAllKeys();
	}
}
