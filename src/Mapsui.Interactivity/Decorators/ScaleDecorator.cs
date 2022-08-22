using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class ScaleDecorator : BaseDecorator
    {
        private readonly MPoint _center;
        private MPoint _scaleTopRight;
        private MPoint _startScaleTopRight;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        private double _startScale;
        // HACK: without this locker Moving() passing not his order
        private bool _isScaling = false;

        public ScaleDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            _scaleTopRight = GetTopRight(featureSource.Geometry!);

            _center = featureSource.Geometry!.Centroid.ToMPoint();

            _startScaleTopRight = _scaleTopRight;

            _startOffsetToVertex = new MPoint();
        }

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            _isScaling = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => new[] { _scaleTopRight };

        public override void Moving(MapInfo? mapInfo)
        {
            if (_isScaling == true && _startGeometry != null && mapInfo?.WorldPosition != null)
            {
                var worldPosition = mapInfo.WorldPosition;

                var p1 = worldPosition - _startOffsetToVertex;

                var scale = _center.Distance(p1);

                var geometry = _startGeometry.Copy();

                Geomorpher.Scale(geometry, scale / _startScale, _center);

                _scaleTopRight = GetTopRight(geometry);

                UpdateGeometry(geometry);
            }
        }

        public override void Starting(MapInfo? mapInfo)
        {
            var worldPosition = mapInfo?.WorldPosition;

            if (worldPosition != null)
            {
                _startScaleTopRight = _scaleTopRight;

                _startOffsetToVertex = worldPosition - _startScaleTopRight;

                _startGeometry = FeatureSource.Geometry!.Copy();

                _startScale = _center.Distance(_startScaleTopRight);

                _isScaling = true;
            }
        }

        public override void Hovering(MapInfo? mapInfo)
        {

        }

        private static MPoint GetTopRight(Geometry geometry)
        {
            var right = geometry.Coordinates.Max(s => s.X);
            var top = geometry.Coordinates.Max(s => s.Y);

            return new MPoint(right, top);
        }
    }
}
