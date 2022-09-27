using Mapsui.Interactivity.Utilities;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class CircleDesigner : BaseDesigner, IAreaDesigner
    {
        private bool _isDrawing = false;
        private bool _firstClick = true;
        private bool _skip;
        private int _counter;
        private List<Coordinate> _featureCoordinates = new();
        protected MPoint? _center;

        internal CircleDesigner() : base() { }

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

                BeginCreating.Execute().Subscribe();

                return;
            }
            else
            {
                EndDrawing();

                _firstClick = true;

                EndCreating.Execute().Subscribe();

                return;
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

        private void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                _center = worldPosition.Copy();

                _featureCoordinates = GetCircle(_center, 0.0, 3);

                Feature = _featureCoordinates.ToPolygon().ToFeature();
                ExtraFeatures = new List<GeometryFeature>();
            }
        }

        private static List<Coordinate> GetCircle(MPoint center, double radius, double quality)
        {
            var centerX = center.X;
            var centerY = center.Y;

            //var radius = Radius.Meters / Math.Cos(Center.Latitude / 180.0 * Math.PI);
            var increment = 360.0 / (quality < 3.0 ? 3.0 : (quality > 360.0 ? 360.0 : quality));
            var vertices = new List<Coordinate>();

            for (double angle = 0; angle < 360; angle += increment)
            {
                var angleRad = angle / 180.0 * Math.PI;
                vertices.Add(new Coordinate(radius * Math.Sin(angleRad) + centerX, radius * Math.Cos(angleRad) + centerY));
            }

            return vertices;
        }

        private void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true && _center != null)
            {
                var p1 = worldPosition.Copy();

                var radius = _center.Distance(p1);

                _featureCoordinates = GetCircle(_center, radius, 180);

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

        public double Area()
        {
            if (Feature.Geometry != null)
            {
                var vertices = _featureCoordinates.Select(s => SphericalMercator.ToLonLat(s.X, s.Y));
                return EarthMath.ComputeSphericalArea(vertices);
            }

            return 0;
        }
    }
}
