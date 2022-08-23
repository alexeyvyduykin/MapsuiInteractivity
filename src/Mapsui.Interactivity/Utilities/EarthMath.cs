namespace Mapsui.Interactivity.Utilities
{
    internal static class EarthMath
    {
        private const double EarthRadius = 6371.009; // 6378.137;
        private const double ToRadians = Math.PI / 180.0;

        public static double ComputeSphericalDistance(IEnumerable<(double lonDeg, double latDeg)> route)
        {
            return ComputeDistance(route, EarthRadius);
        }

        private static double ComputeDistance(IEnumerable<(double, double)> route, double radius)
        {
            int size = route.Count();

            if (size < 2)
            {
                return 0;
            }

            double total = 0;

            var prevPoint = route.First();

            foreach (var point in route.Skip(1))
            {
                double dist = GetDistance(prevPoint, point, radius);

                total += dist;
                prevPoint = point;
            }

            return total;
        }

        private static double GetDistance((double lon, double lat) p0, (double lon, double lat) p1, double radius)
        {
            var lat0 = p0.lat * ToRadians;
            var lon0 = p0.lon * ToRadians;
            var lat1 = p1.lat * ToRadians;
            var lon1 = p1.lon * ToRadians;
            var dlon = lon1 - lon0;
            var d = Math.Pow(Math.Sin((lat1 - lat0) / 2.0), 2.0) + Math.Cos(lat0) * Math.Cos(lat1) * Math.Pow(Math.Sin(dlon / 2.0), 2.0);
            return radius * (2.0 * Math.Atan2(Math.Sqrt(d), Math.Sqrt(1.0 - d)));
        }

        public static double ComputeSphericalArea(IEnumerable<(double lonDeg, double latDeg)> vertices)
        {
            return Math.Abs(ComputeSignedArea(vertices, EarthRadius));
        }

        private static double ComputeSignedArea(IEnumerable<(double lon, double lat)> vertices, double radius)
        {
            int size = vertices.Count();

            if (size < 3)
            {
                return 0;
            }

            double total = 0;
            var prev = vertices.Last();
            double prevTanLat = Math.Tan((Math.PI / 2 - prev.lat * ToRadians) / 2);
            double prevLng = prev.lon * ToRadians;

            foreach (var (lon, lat) in vertices)
            {
                double tanLat = Math.Tan((Math.PI / 2 - lat * ToRadians) / 2);
                double lng = lon * ToRadians;
                total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
                prevTanLat = tanLat;
                prevLng = lng;
            }

            return total * (radius * radius);
        }

        private static double PolarTriangleArea(double tan1, double lng1, double tan2, double lng2)
        {
            double deltaLng = lng1 - lng2;
            double t = tan1 * tan2;
            return 2 * Math.Atan2(t * Math.Sin(deltaLng), 1 + t * Math.Cos(deltaLng));
        }
    }
}
