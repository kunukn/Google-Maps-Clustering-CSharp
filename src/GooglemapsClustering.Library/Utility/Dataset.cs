using System;
using System.Collections.Generic;
using System.IO;
using GooglemapsClustering.Clustering.Data.Geometry;

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
            var fi = new FileInfo(websitepath);
            if (!fi.Exists)
            {
                throw new ApplicationException("File does not exists: " + fi.FullName);
            }

            var list = FileUtil.ReadFile(filepath);
            var dataset = new List<P>();

            foreach (var s in list)
            {
                var arr = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length != 4) continue;

                var x = arr[0].ToDouble();
                var y = arr[1].ToDouble();
                var i = arr[2].ToInt();
                var t = arr[3].ToInt();

                var p = new P {X = x, Y = y, I = i, T = t};
                p.Normalize();
                dataset.Add(p);
            }            
            return dataset;
        }
    }
}