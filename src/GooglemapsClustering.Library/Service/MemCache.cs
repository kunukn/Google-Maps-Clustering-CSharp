using System;
using System.Collections.Generic;
using System.Linq;
using GooglemapsClustering.Clustering.Contract;
using System.Runtime.Caching;

namespace GooglemapsClustering.Clustering.Service
{
	/// <summary>
	/// http://msdn.microsoft.com/library/system.runtime.caching.memorycache.aspx
	/// </summary>
	public class MemCache : IMemCache
	{
		private  readonly ObjectCache _cache = MemoryCache.Default;

		public  T Get<T>(string key) where T : class
		{
			return _cache[key] as T;			
		}

		public  bool Add<T>(T objectToCache, string key, TimeSpan timespan)
		{
			return _cache.Add(key, objectToCache, DateTime.Now.Add(timespan));
		}

		public  object Clear(string key)
		{
			return _cache.Remove(key);
		}
		public  bool Exists(string key)
		{
			return _cache.Get(key) != null;
		}

		public  IList<string> GetAllKeys()
		{
			return _cache.Select(keyValuePair => keyValuePair.Key).ToList();
		}		
	}
}
