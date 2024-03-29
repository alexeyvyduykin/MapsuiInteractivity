﻿using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity;

public class RectangleDesigner : BaseDesigner, IDesigner
{
    private bool _isDrawing = false;
    private bool _firstClick = true;
    private bool _skip;
    private int _counter;
    private List<Coordinate> _featureCoordinates = new();

    internal RectangleDesigner() : base() { }

    public override IEnumerable<MPoint> GetActiveVertices() => Array.Empty<MPoint>();

    public override void Starting(MapInfo? mapInfo)
    {
        _skip = false;
        _counter = 0;
    }

    public override void Moving(MapInfo? mapInfo)
    {
        if (_counter++ > 0)
        {
            _skip = true;
        }
    }

    public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
    {
        if (_skip == false)
        {
            CreatingFeature(mapInfo?.WorldPosition!);
        }
    }

    public override void Hovering(MapInfo? mapInfo)
    {
        HoverCreatingFeature(mapInfo?.WorldPosition!);
    }

    private void CreatingFeature(MPoint worldPosition)
    {
        if (_firstClick == true)
        {
            BeginDrawing(worldPosition);

            _firstClick = false;

            OnBeginCreating();

            return;
        }
        else
        {
            EndDrawing();

            _firstClick = true;

            OnEndCreating();

            return;
        }
    }

    private void HoverCreatingFeature(MPoint worldPosition)
    {
        if (_firstClick == false)
        {
            DrawingHover(worldPosition);

            OnHoverCreating();

            OnInvalidate();
        }
    }

    private void BeginDrawing(MPoint worldPosition)
    {
        if (_isDrawing == false)
        {
            _isDrawing = true;

            var p0 = worldPosition.ToCoordinate();
            var p1 = worldPosition.ToCoordinate();
            var p2 = worldPosition.ToCoordinate();
            var p3 = worldPosition.ToCoordinate();

            _featureCoordinates = new() { p0, p1, p2, p3 };
            Feature = _featureCoordinates.ToPolygon().ToFeature();
            ExtraFeatures = new List<GeometryFeature>();
        }
    }

    private void DrawingHover(MPoint worldPosition)
    {
        if (_isDrawing == true)
        {
            var p2 = worldPosition.ToCoordinate();
            var p0 = _featureCoordinates[0];
            var p1 = (p2.X, p0.Y).ToCoordinate();
            var p3 = (p0.X, p2.Y).ToCoordinate();

            _featureCoordinates = new() { p0, p1, p2, p3 };
            Feature.Geometry = _featureCoordinates.ToPolygon();

            Feature.RenderedGeometry?.Clear();
        }
    }

    private void EndDrawing()
    {
        if (_isDrawing == true)
        {
            _isDrawing = false;
        }
    }
}
