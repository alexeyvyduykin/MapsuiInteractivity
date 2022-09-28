using Mapsui.Interactivity.Utilities;
using Mapsui.Projections;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity.Extensions
{
    public static class DesignerExtensions
    {
        public static double Area(this Geometry geometry)
        {
            var arr = geometry.MainVertices();

            if (arr != null)
            {
                var vertices = arr.Select(s => SphericalMercator.ToLonLat(s.X, s.Y));
                return EarthMath.ComputeSphericalArea(vertices);
            }

            return 0;
        }

        public static double Area(this IDesigner designer)
        {
            return designer.Feature.Geometry?.Area() ?? 0.0;
        }

        public static double Distance(this Geometry geometry)
        {
            var verts = geometry.Coordinates;
            var vertices = verts.Select(s => SphericalMercator.ToLonLat(s.X, s.Y));
            return EarthMath.ComputeSphericalDistance(vertices);
        }

        public static double Distance(this IDesigner designer)
        {
            if (designer.Feature.Geometry != null)
            {
                var verts0 = designer.Feature.Geometry.Coordinates;
                var verts1 = designer.ExtraFeatures.Single().Geometry!.Coordinates;
                var verts = verts0.Union(verts1);
                var vertices = verts.Select(s => SphericalMercator.ToLonLat(s.X, s.Y));
                return EarthMath.ComputeSphericalDistance(vertices);
            }

            return 0;
        }
    }
}
