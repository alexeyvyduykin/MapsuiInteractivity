using Mapsui;
using Mapsui.Utilities;
using NetTopologySuite.Geometries;

namespace InteractivityWPFSample.Models;

public static class Geomorpher
{
    public static void Rotate(Geometry geometry, double degrees, MPoint center)
    {
        foreach (var coordinate in geometry.Coordinates)
        {
            Rotate(coordinate, degrees, center);
        }
    }

    private static void Rotate(Coordinate vertex, double degrees, MPoint center)
    {
        // translate this point back to the center
        var newX = vertex.X - center.X;
        var newY = vertex.Y - center.Y;

        // rotate the values
        var p = Algorithms.RotateClockwiseDegrees(newX, newY, degrees);

        // translate back to original reference frame
        vertex.X = p.X + center.X;
        vertex.Y = p.Y + center.Y;
    }

    public static void Scale(Geometry geometry, double scale, MPoint center)
    {
        foreach (var coordinate in geometry.Coordinates)
        {
            Scale(coordinate, scale, center);
        }
    }

    private static void Scale(Coordinate vertex, double scale, MPoint center)
    {
        vertex.X = center.X + (vertex.X - center.X) * scale;
        vertex.Y = center.Y + (vertex.Y - center.Y) * scale;
    }

    public static void Translate(Geometry geometry, double deltaX, double deltaY)
    {
        foreach (var vertex in geometry.Coordinates)
        {
            Translate(vertex, deltaX, deltaY);
        }
    }

    public static void Translate(Coordinate vertex, double deltaX, double deltaY)
    {
        vertex.X += deltaX;
        vertex.Y += deltaY;
    }
}