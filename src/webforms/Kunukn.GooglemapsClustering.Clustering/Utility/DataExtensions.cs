using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kunukn.GooglemapsClustering.Clustering.Contract;
using Kunukn.GooglemapsClustering.Clustering.Data;

namespace Kunukn.GooglemapsClustering.Clustering.Utility
{
    public static class DataExtensions
    {
        static readonly CultureInfo CultureEnUs = new CultureInfo("en-US");
        const string S = "G";

        const double Pi2 = Math.PI * 2;
        public const int RoundConvertError = 5;

        public static double Round(this double d)
        {
            return Math.Round(d, Numbers.Round);
        }
        
        public static void Normalize(this IPoints list)
        {
            foreach (var p in list.Data) p.Normalize();
        }

        // Distance
        public static double AbsLat(this double beg, double end)
        {
            double b = beg;
            double e = end;
            if (b > e)
            {
                e += LatLonInfo.MaxLatLength;
            }
                
            double diff = e - b;
            if (diff < 0 || diff > LatLonInfo.MaxLatLength)
            {
                throw new ApplicationException(string.Format("Error AbsLat beg: {0} end: {1}", beg, end));
            }
                
            return diff;
        }

        // Distance
        public static double AbsLon(this double beg, double end)
        {
            double b = beg;
            double e = end;
            if (b > e)
            {
                e += LatLonInfo.MaxLonLength;
            }                
            double diff = e - b;
            if (diff < 0 || diff > LatLonInfo.MaxLonLength)
            {
                throw new ApplicationException(string.Format("Error AbsLon beg: {0} end: {1}", beg, end));
            }
                
            return diff;
        }
       
        // positive version of lat, lon
        public static double Pos(this double latlon)
        {
            if (latlon < LatLonInfo.MinLonValue || latlon > LatLonInfo.MaxLonValue)
            {
                throw new ApplicationException("Pos");
            }
                
            if (latlon < 0)
            {
                return latlon + LatLonInfo.MaxWorldLength;
            }

            return latlon;
        }


        // Lat or Lon
        public static double LatLonToDegree(this double latlon)
        {
            if (latlon < LatLonInfo.MinLonValue || latlon > LatLonInfo.MaxLonValue)
            {
                throw new ApplicationException("LatLonToDegree");
            }
                
            return (latlon + LatLonInfo.AngleConvert + LatLonInfo.MaxWorldLength) % LatLonInfo.MaxWorldLength;
        }
        public static double DegreeToLatLon(this double degree)
        {
            if (degree < 0 || degree > 360)
            {
                throw new ApplicationException("DegreeToLatLon");
            }
                
            return (degree - LatLonInfo.AngleConvert);
        }

        public static double LatLonToRadian(this double latlon)
        {
            if (latlon < LatLonInfo.MinLonValue || latlon > LatLonInfo.MaxLonValue)
            {
                throw new ApplicationException("LatLonToRadian");
            }
                
            var degree = LatLonToDegree(latlon);
            var radian = DegreeToRadian(degree);
            return radian;
        }

        public static double RadianNormalize(this double r)
        {
            if (r < -Pi2 || r > Pi2)
            {
                throw new ApplicationException("RadianNormalize");
            }
                
            var radian = (r + Pi2) % Pi2;
            if (radian < 0 || radian > Pi2)
            {
                throw new ApplicationException("RadianNormalize");
            }
                
            return radian;
        }

        public static double DegreeNormalize(this double d)
        {
            if (d < -360 || d > 360)
            {
                throw new ApplicationException("DegreeNormalize");
            }
                
            var degree = (d + 360) % 360;
            if (degree < 0 || degree > 360)
            {
                throw new ApplicationException("DegreeNormalize");
            }
                
            return degree;
        }


        public static double RadianToLatLon(this double r)
        {
            var radian = RadianNormalize( r);
            if (radian < 0 || radian > Pi2)
            {
                throw new ApplicationException("RadianToLatLon");
            }
                
            var degree =  DegreeNormalize(RadianToDegree(radian) );
            var degreeRounded = Math.Round(degree, RoundConvertError);            
            var latlon = DegreeToLatLon(degreeRounded);
            return latlon;
        }

        public static double RadianToDegree(this double radian)
        {
            if (radian < 0 || radian > Pi2)
            {
                throw new ApplicationException("RadianToDegree");
            }
                
            return (radian / Math.PI) * 180.0;
        }
        public static double DegreeToRadian(this double degree)
        {
            if (degree < 0 || degree > 360)
            {
                throw new ApplicationException("DegreeToRadian");
            }
                
            return (degree * Math.PI) / 180.0;
        }


        // ]-180;180]
        // lon wrap around at value -180 and 180, nb. -180 = 180
        public static double NormalizeLongitude(this double lon)
        {
            // naive version
            //while(normalized<MinValue) ... normalized += MaxValue; 

            double normalized = lon;
            if (lon < LatLonInfo.MinLonValue)
            {
                var m = lon % LatLonInfo.MinLonValue;
                normalized = LatLonInfo.MaxLonValue + m;
            }
            else if (lon > LatLonInfo.MaxLonValue)
            {
                var m = lon % LatLonInfo.MaxLonValue;
                normalized = LatLonInfo.MinLonValue + m;
            }
            
            return normalized;
        }

        // [-90;90]
        // -90 is south pole, 90 is north pole thus -90 != 90
        // no wrap, because google map dont wrap on lat
        public static double NormalizeLatitude(this double lat)
        {
            double normalized = lat;
            if (lat < LatLonInfo.MinLatValue)
            {
                //var m = lat % -LatLonInfo.MaxLatValue;
                //normalized = LatLonInfo.MaxLatValue + m;
                normalized = LatLonInfo.MinLatValue;
            }
            if (lat > LatLonInfo.MaxLatValue)
            {
                //var m = lat % LatLonInfo.MaxLatValue;
                //normalized = -LatLonInfo.MaxLatValue + m;                
                normalized = LatLonInfo.MaxLatValue;
            }

            return normalized;
        }

        public static double ToDouble(this string s)
        {
            return double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static string DoubleToString(this double d)
        {
            double rounded = Math.Round(d, Numbers.Round);
            return rounded.ToString(S, CultureEnUs);
        }       

        public static string ListToString<T>(this List<T> list  )
        {
            return list.Aggregate("", (a, b) => a + "[" + b + "]\n");            
        }

        public static int[] ToNumbers(this string s)
        {
            if(string.IsNullOrWhiteSpace(s)) return new int[]{};

            var arr = s.Split(new [] {";"}, StringSplitOptions.RemoveEmptyEntries);
            var ints = new int[arr.Length];
            for (var i = 0; i < arr.Length; i++) ints[i] = int.Parse(arr[i]);

            return ints;
        }
    }
}