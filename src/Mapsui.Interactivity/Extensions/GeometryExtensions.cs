using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity;

public static class GeometryExtensions
{
    public static IList<Point> MainVertices(this Geometry geometry)
    {
        if (geometry is LineString lineString)
        {
            return lineString.Coordinates.Select(s => s.ToPoint()).ToList();// Vertices;
        }
        if (geometry is Polygon polygon)
        {
            return polygon.ExteriorRing?.Coordinates.Select(s => s.ToPoint()).ToList() ?? new List<Point>();
        }
        if (geometry is Point point)
        {
            return new List<Point> { point };
        }
        throw new NotImplementedException();
    }

    public static IList<Coordinate> MainCoordinates(this Geometry geometry)
    {
        if (geometry is LineString lineString)
        {
            return lineString.Coordinates;
        }
        if (geometry is Polygon polygon)
        {
            return polygon.ExteriorRing?.Coordinates.ToList() ?? new List<Coordinate>();
        }
        if (geometry is Point point)
        {
            return new List<Coordinate> { new Coordinate(point.X, point.Y) };
        }
        throw new NotImplementedException();
    }

    public static GeometryFeature ToFeature(this Geometry geometry, string name)
    {
        var feature = geometry.ToFeature();

        feature["Name"] = name;

        return feature;
    }
}
