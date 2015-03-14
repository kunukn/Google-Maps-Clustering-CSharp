using System;
using System.Collections.Generic;
using System.IO;
using GooglemapsClustering.Clustering.Data.Geometry;
using GooglemapsClustering.Clustering.Extensions;

namespace GooglemapsClustering.Clustering.Utility
{
	/// <summary>
	/// Author: Kunuk Nykjaer
	/// </summary>
	public class Dataset
	{
		// Database simulation
		public static List<P> LoadDataset(string websitepath)
		{
			return LoadDatasetFromCsv(websitepath);
		}

		private static List<P> LoadDatasetFromCsv(string websitepath)
		{
			var filepath = websitepath;
			var fileInfo = new FileInfo(websitepath);
			if (!fileInfo.Exists)
			{
				throw new ApplicationException(string.Concat("File does not exists: ", fileInfo.FullName));
			}

			var lines = FileUtil.ReadFile(filepath);
			var dataset = new List<P>();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				if (line.TrimStart().StartsWith("#")) continue;

				var arr = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (arr.Length != 4) continue;

				var x = arr[0].ToDouble(); // lon
				var y = arr[1].ToDouble(); // lat
				var id = arr[2].ToInt();
				var type = arr[3].ToInt();

				var p = new P { X = x, Y = y, I = id, T = type };
				p.Normalize();
				dataset.Add(p);
			}
			return dataset;
		}
	}
}