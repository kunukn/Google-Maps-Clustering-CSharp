using System;
using GooglemapsClustering.Clustering.Data;

namespace GooglemapsClustering.Clustering.Utility
{
    /// <summary>
    /// Author: Kunuk Nykjaer
    /// </summary>
    public static class MathTool
    {
        const double Exp = 2; // 2=euclid, 1=manhatten
        // Minkowski dist        
        // if lat lon precise dist is needed, use Haversine or similar formulas
        // this is approx calc for clustering, no precise dist is needed
        public static double Distance(P a, P b)
        {
            // lat lon wrap, values don't seem needed to be normalized to [0;1] for better distance calc
            var absx = LatLonDiff(a.X, b.X);
            var absy = LatLonDiff(a.Y, b.Y);

            return Math.Pow(Math.Pow(absx, Exp) +
                Math.Pow(Math.Abs(absy), Exp), 1.0 / Exp);
        }

        // O(1) while loop is maximum 2
        public static double LatLonDiff(double from, double to)
        {
            double difference = to - from;
            while (difference < -LatLonInfo.MaxLengthWrap) difference += LatLonInfo.MaxWorldLength;
            while (difference > LatLonInfo.MaxLengthWrap) difference -= LatLonInfo.MaxWorldLength;
            return Math.Abs(difference);

            //var differenceAngle = (to - from) % 180; //not working for -170 to 170
            //return Math.Abs(differenceAngle);
        }

        public static double Haversine(P p1, P p2)
        {
            return Haversine(p1.Y, p1.X, p2.Y, p2.X);
        }

        // http://en.wikipedia.org/wiki/Haversine_formula
        // Approx dist between two points on earth
        //public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        //{
        //    const int R = 6371; // km
        //    var dLat = ToRadians(lat2 - lat1);
        //    var dLon = ToRadians(lon2 - lon1);
        //    lat1 = ToRadians(lat1);
        //    lat2 = ToRadians(lat2);

        //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    var d = R * c;
        //    return d;
        //}
        // http://en.wikipedia.org/wiki/Haversine_formula
        // Approx dist between two points on earth
        // http://rosettacode.org/wiki/Haversine_formula
        public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6372.8; // In kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            var d = R * c;
            return d;
        }

        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

      
        public static bool IsLowerThanLatMin(double d)
        {
            return d < LatLonInfo.MinLatValue;
        }
        public static bool IsGreaterThanLatMax(double d)
        {
            return d > LatLonInfo.MaxLatValue;
        }

        /// <summary>
        /// Lat Lon specific rect boundary check, is x,y inside boundary?
        /// </summary>
        /// <param name="minx"></param>
        /// <param name="miny"></param>
        /// <param name="maxx"></param>
        /// <param name="maxy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// /// <param name="isInsideDetectedX"></param>
        /// /// <param name="isInsideDetectedY"></param>
        /// <returns></returns>
        public static bool IsInside(double minx, double miny, double maxx, double maxy, double x, double y, bool isInsideDetectedX, bool isInsideDetectedY)
        {
            // Normalize because of widen function, world wrapping might have occured
            // calc in positive value range only, nb. lon -170 = 10, lat -80 = 10
            var nminx = minx.NormalizeLongitude();
            var nmaxx = maxx.NormalizeLongitude();

            var nminy = miny.NormalizeLatitude();
            var nmaxy = maxy.NormalizeLatitude();

            var nx = x.NormalizeLongitude();
            var ny = y.NormalizeLatitude();

            bool isX = isInsideDetectedX; // skip checking?
            bool isY = isInsideDetectedY;

            if (!isInsideDetectedY)
            {
                // world wrap y
                if (nminy > nmaxy)
                {
                    //sign depended check, todo merge equal lines
                    // - -
                    if (nmaxy <= 0 && nminy <= 0)
                    {
                        isY = nminy <= ny && ny <= LatLonInfo.MaxLatValue || LatLonInfo.MinLatValue <= ny && ny <= nmaxy;
                    }                        
                    // + +
                    else if (nmaxy >= 0 && nminy >= 0)
                    {
                        isY = nminy <= ny && ny <= LatLonInfo.MaxLatValue || LatLonInfo.MinLatValue <= ny && ny <= nmaxy;
                    }                        
                    // + -
                    else
                    {
                        isY = nminy <= ny && ny <= LatLonInfo.MaxLatValue || LatLonInfo.MinLatValue <= ny && ny <= nmaxy;
                    }                        
                }

                else
                {
                    // normal, no world wrap 
                    isY = nminy <= ny && ny <= nmaxy;
                }
            }

            if (!isInsideDetectedX)
            {
                // world wrap x
                if (nminx > nmaxx)
                {
                    //sign depended check, todo merge equal lines
                    // - -
                    if (nmaxx <= 0 && nminx <= 0)
                    {
                        isX = nminx <= nx && nx <= LatLonInfo.MaxLonValue || LatLonInfo.MinLonValue <= nx && nx <= nmaxx;
                    }                        
                    // + +
                    else if (nmaxx >= 0 && nminx >= 0)
                    {
                        isX = nminx <= nx && nx <= LatLonInfo.MaxLonValue || LatLonInfo.MinLonValue <= nx && nx <= nmaxx;
                    }                        
                    // + -
                    else
                    {
                        isX = nminx <= nx && nx <= LatLonInfo.MaxLonValue || LatLonInfo.MinLonValue <= nx && nx <= nmaxx;
                    }                        
                }
                else
                {
                    // normal, no world wrap                 
                    isX = nminx <= nx && nx <= nmaxx;
                }
            }

            return isX && isY;
        }

        public static bool IsInside(Boundary b, P p)
        {
            return IsInside(b.Minx, b.Miny, b.Maxx, b.Maxy, p.X, p.Y, false, false);
        }
        
        // used by zoom level and deciding the grid size, O(halfSteps)
        // O(halfSteps) ~  O(maxzoom) ~  O(k) ~  O(1)
        // Google Maps doubles or halves the view for 1 step zoom level change
        public static double Half(double d, int halfSteps)
        {
            // http://en.wikipedia.org/wiki/Decimal_degrees
            const double meter11 = 0.0001; //decimal degrees

            double half = d;
            for (int i = 0; i < halfSteps; i++)
            {
                half /= 2;
            }
                
            var halfRounded = Math.Round(half, 4);
            // avoid grid span less than this level
            return halfRounded < meter11 ? meter11 : halfRounded;
        }

        // Value x which is in range [a,b] is mapped to a new value in range [c;d]
        public static double Map(double x, double a, double b, double c, double d)
        {
            var r = (x - a) / (b - a) * (d - c) + c;
            return r;
        }

        // Grid location are stationary, this gives first left or lower grid line from current latOrLon
        public static double FloorLatLon(double latOrlon, double delta)
        {
            var floor = ((int)(latOrlon / delta)) * delta;
            if (latOrlon < 0) floor -= delta;

            return floor;
        }

        // 
        public static bool IsLatValid(double d)
        {
            return LatLonInfo.MinLatValue <= d && d <= LatLonInfo.MaxLatValue;
        }
        public static bool IsLonValid(double d)
        {
            return LatLonInfo.MinLonValue <= d && d <= LatLonInfo.MaxLonValue;
        }

        // Value must be within a and b
        public static double Constrain(double x, double a, double b)
        {
            var r = Math.Max(a, Math.Min(x, b));
            return r;
        }

        // Value must be within latitude boundary        
        public static double ConstrainLatitude(double x, double offset = 0)
        {            
            var r = Math.Max(LatLonInfo.MinLatValue + offset, Math.Min(x, LatLonInfo.MaxLatValue - offset));
            return r;
        }        
    }
}
