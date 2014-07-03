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
		private  readonly ObjectCache Cache = MemoryCache.Default;

		public  T Get<T>(string key) where T : class
		{
			try	{return Cache[key] as T;}
			catch {return null;}
		}

		public  bool Add(object objectToCache, string key, TimeSpan timespan)
		{
			return Cache.Add(key, objectToCache, DateTime.Now.Add(timespan));
		}

		public  object Clear(string key)
		{
			return Cache.Remove(key);
		}
		public  bool Exists(string key)
		{
			return Cache.Get(key) != null;
		}

		public  IList<string> GetAllKeys()
		{
			return Cache.Select(keyValuePair => keyValuePair.Key).ToList();
		}		
	}
}
