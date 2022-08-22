using Mapsui.Nts;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class RotateDecorator : BaseDecorator
    {
        private readonly MPoint _center;
        private MPoint _rotateRight;
        private MPoint _startRotateRight;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        private double _halfDiagonal;
        // HACK: without this locker Moving() passing not his order
        private bool _isRotating = false;

        public RotateDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            var extent = GetExtent(featureSource);

            _rotateRight = new MPoint(extent.Right, extent.Centroid.Y);

            _center = extent.Centroid;

            _startRotateRight = _rotateRight;

            _halfDiagonal = Diagonal(extent) / 2.0;

            _startOffsetToVertex = new MPoint();
        }

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            var extent = GetExtent(FeatureSource);

            _rotateRight = new MPoint(extent.Right, extent.Centroid.Y);

            _isRotating = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => new[] { _rotateRight };

        public override void Moving(MPoint worldPosition)
        {
            if (_isRotating == true && _startGeometry != null)
            {
                var p1 = worldPosition - _startOffsetToVertex;

                var distance = _startRotateRight.Distance(p1);

                var sign = (p1 - _startRotateRight).Y >= 0 ? -1 : 1;

                var geometry = _startGeometry.Copy();

                var degrees = sign * (distance * 360.0 / _halfDiagonal);

                Geomorpher.Rotate(geometry, degrees, _center);

                _rotateRight = new MPoint(_startRotateRight.X, p1.Y);

                UpdateGeometry(geometry);
            }
        }

        public override void Starting(MPoint worldPosition)
        {
            _startRotateRight = _rotateRight;

            _startOffsetToVertex = worldPosition - _startRotateRight;

            _startGeometry = FeatureSource.Geometry!.Copy();

            var extent = GetExtent(FeatureSource);

            _halfDiagonal = Diagonal(extent) / 2.0;

            _isRotating = true;
        }

        public override void Hovering(MapInfo? mapInfo)
        {

        }

        private static double Diagonal(MRect box)
        {
            return Math.Sqrt(box.Width * box.Width + box.Height * box.Height);
        }

        private static MRect GetExtent(GeometryFeature feature)
        {
            var minX = feature.Geometry!.Coordinates.Min(s => s.X);
            var minY = feature.Geometry!.Coordinates.Min(s => s.Y);
            var maxX = feature.Geometry!.Coordinates.Max(s => s.X);
            var maxY = feature.Geometry!.Coordinates.Max(s => s.Y);

            return new MRect(minX, minY, maxX, maxY);
        }

        public override void Dispose(MapInfo? mapInfo)
        {
            EndDecoratingCallback();
        }
    }
}
