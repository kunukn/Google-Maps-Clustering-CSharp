namespace GooglemapsClustering.Clustering.Data
{
    public static class LatLonInfo
    {
        public const double MinLatValue = -90;
        public const double MaxLatValue = 90;
        public static readonly double MinLonValue = -180;
        public const double MaxLonValue = 180;

        public const double MaxLatLength = 180;
        public const double MaxLonLength = 360;
        
        public const double MaxLengthWrap = 180;
        public const double MaxWorldLength = 360;

        public const double AngleConvert = 180;

        public const int Lat = 0;
        public const int Lon = 1;
    }
}
