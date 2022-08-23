using Mapsui.Interactivity.Utilities;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class TranslateDecorator : BaseDecorator
    {
        private MPoint _center;
        private MPoint _startCenter;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        // HACK: without this locker Moving() passing not his order
        private bool _isTranslating = false;

        public TranslateDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            _center = featureSource.Geometry!.Centroid.ToMPoint();

            _startCenter = _center;

            _startOffsetToVertex = new MPoint();
        }

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            _isTranslating = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => new[] { _center };

        public override void Moving(MapInfo? mapInfo)
        {
            if (_isTranslating == true && _startGeometry != null && mapInfo?.WorldPosition != null)
            {
                var worldPosition = mapInfo.WorldPosition;

                var p1 = worldPosition - _startOffsetToVertex;

                var delta = p1 - _startCenter;

                var geometry = _startGeometry.Copy();

                Geomorpher.Translate(geometry, delta.X, delta.Y);

                _center = geometry.Centroid.ToMPoint();

                UpdateGeometry(geometry);
            }
        }

        public override void Starting(MapInfo? mapInfo)
        {
            var worldPosition = mapInfo?.WorldPosition;

            if (worldPosition != null)
            {
                _startCenter = _center;

                _startOffsetToVertex = worldPosition - _startCenter;

                _startGeometry = FeatureSource.Geometry!.Copy();

                _isTranslating = true;
            }
        }
    }
}
