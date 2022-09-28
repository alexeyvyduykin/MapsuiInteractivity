using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class RouteDesigner : BaseDesigner, IDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;
        private bool _firstClick = true;
        private GeometryFeature? _extraLineString;
        private List<Coordinate> _featureCoordinates = new();

        internal RouteDesigner() : base() { }

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            if (Feature.Geometry != null)
            {
                return Feature.Geometry.MainVertices().Select(s => s.ToMPoint());
            }

            return Array.Empty<MPoint>();
        }

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
                CreatingFeature(mapInfo?.WorldPosition!, isEnd);
            }
        }

        public override void Hovering(MapInfo? mapInfo)
        {
            HoverCreatingFeature(mapInfo?.WorldPosition!);
        }

        private void CreatingFeature(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            if (_firstClick == true)
            {
                BeginDrawing(worldPosition);

                _firstClick = false;

                BeginCreating.Execute().Subscribe();

                return;
            }
            else
            {
                var res = IsEndDrawing(isEnd);

                if (res == true)
                {
                    EndDrawing();

                    _firstClick = true;

                    EndCreating.Execute().Subscribe();

                    return;
                }
                else
                {
                    Drawing(worldPosition);

                    Creating.Execute().Subscribe();

                    return;
                }
            }
        }

        private void HoverCreatingFeature(MPoint worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                HoverCreating.Execute().Subscribe();

                Invalidate.Execute().Subscribe();
            }
        }

        private bool IsEndDrawing(Predicate<MPoint>? isEnd)
        {
            var routeGeometry = (LineString)Feature.Geometry!;

            if (routeGeometry.Coordinates.Length > 1)
            {
                foreach (var item in routeGeometry.Coordinates)
                {
                    var click = isEnd?.Invoke(item.ToMPoint());
                    if (click == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                return;
            }

            _isDrawing = true;

            var p0 = worldPosition.ToCoordinate();
            var p1 = worldPosition.ToCoordinate();

            _extraLineString = new[] { p0, p1 }.ToLineString().ToFeature("ExtraRouteHoverLine");

            _featureCoordinates = new() { p0 };
            Feature = _featureCoordinates.ToLineString().ToFeature();
            ExtraFeatures = new List<GeometryFeature>() { _extraLineString };
        }

        private void Drawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString!.Geometry!).EndPoint.ToMPoint().ToCoordinate();
                var p1 = worldPosition.ToCoordinate();
                var p2 = worldPosition.ToCoordinate();

                _featureCoordinates.Add(p0);
                Feature.Geometry = _featureCoordinates.ToLineString();

                _extraLineString.Geometry = new[] { p1, p2 }.ToLineString();

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
            }
        }

        private void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_extraLineString!.Geometry!).EndPoint.X = worldPosition.X;
                ((LineString)_extraLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _extraLineString.RenderedGeometry?.Clear();
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
}
