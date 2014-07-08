using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GooglemapsClustering.Clustering.Extensions;

namespace GooglemapsClustering.Clustering.Utility
{
	/// <summary>
	/// Author: Kunuk Nykjaer
	/// </summary>
	public static class FileUtil
	{		
		public const string FolderPath = @"c:\temp\";
		private static readonly Encoding EncodingRead = Encoding.Default; // Encoding.Default  Encoding.UTF8  Encoding.Unicode      
		private static readonly Encoding EncodingWrite = Encoding.Unicode; //Encoding.Default  Encoding.UTF8  Encoding.Unicode

		/// <summary>        
		/// folder path is created if not exists
		/// </summary>
		public static void WriteFile(List<string> data, FileInfo fileInfo)
		{
			var sb = new StringBuilder();
			var i = 0;
			var len = data.Count;
			foreach (var line in data)
			{
				sb.Append(line);
				i++;
				if (i < len) sb.Append(Environment.NewLine);
			}
			WriteFile(sb.ToString(), fileInfo);
		}
		public static void WriteFile(string data, FileInfo fileInfo)
		{
			if (fileInfo._(_ => _.Directory) == null) throw new ArgumentException("fileInfo");
			if (!fileInfo.Directory.Exists) Directory.CreateDirectory(fileInfo.Directory.ToString());
			
			using (var streamWriter = fileInfo.CreateText()) streamWriter.Write(data, EncodingWrite);			
		}

		/// <summary>
		/// Creates the folders if not exists for file path
		/// </summary>
		public static void CreateFilePath(string filepath)
		{
			var fi = new FileInfo(filepath);

			if (fi.Directory == null) throw new ArgumentException("filepath", filepath);
			if (!fi.Directory.Exists) Directory.CreateDirectory(fi.Directory.ToString());			
		}

		public static List<string> ReadFile(string path)
		{
			return System.IO.File.ReadAllLines(path, EncodingRead).ToList();
		}
	}
}
