using Mapsui.Interactivity.Utilities;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class EditDecorator : BaseDecorator
    {
        private IList<MPoint> _points;
        private MPoint? _startPoint;
        private int _index;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        private readonly bool _isRectangle;
        // HACK: without this locker Moving() passing not his order
        private bool _isEditing = false;

        internal EditDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            _points = featureSource.Geometry!.MainVertices().Select(s => s.ToMPoint()).ToList();

            _startOffsetToVertex = new MPoint();

            _isRectangle = IsRectangle(_points);
        }

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            _isEditing = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => _points;

        public override void Moving(MapInfo? mapInfo)
        {
            if (_isEditing == true && _startGeometry != null && mapInfo?.WorldPosition != null)
            {
                var worldPosition = mapInfo.WorldPosition;

                var p1 = worldPosition - _startOffsetToVertex;

                var delta = p1 - _startPoint!;

                var geometry = _startGeometry.Copy();

                var pp = geometry.Coordinates[_index];
                Geomorpher.Translate(pp, delta.X, delta.Y);

                if (geometry is Polygon)
                {
                    if (_index == 0 || _index == _points.Count - 1)
                    {
                        var ppp = geometry.Coordinates[_index == 0 ? _points.Count - 1 : 0];
                        Geomorpher.Translate(ppp, delta.X, delta.Y);
                    }
                }

                if (_isRectangle == true)
                {
                    if (_index == 0 || _index == 4)
                    {
                        Geomorpher.Translate(geometry.Coordinates[1], 0.0, delta.Y);
                        Geomorpher.Translate(geometry.Coordinates[3], delta.X, 0.0);
                    }
                    else if (_index == 1)
                    {
                        Geomorpher.Translate(geometry.Coordinates[0], 0.0, delta.Y);
                        Geomorpher.Translate(geometry.Coordinates[2], delta.X, 0.0);
                        Geomorpher.Translate(geometry.Coordinates[4], 0.0, delta.Y);
                    }
                    else if (_index == 2)
                    {
                        Geomorpher.Translate(geometry.Coordinates[1], delta.X, 0.0);
                        Geomorpher.Translate(geometry.Coordinates[3], 0.0, delta.Y);
                    }
                    else if (_index == 3)
                    {
                        Geomorpher.Translate(geometry.Coordinates[0], delta.X, 0.0);
                        Geomorpher.Translate(geometry.Coordinates[2], 0.0, delta.Y);
                        Geomorpher.Translate(geometry.Coordinates[4], delta.X, 0.0);
                    }
                }

                _points = geometry.MainVertices().Select(s => s.ToMPoint()).ToList();

                UpdateGeometry(geometry);
            }
        }

        public override void Starting(MapInfo? mapInfo)
        {
            var worldPosition = mapInfo?.WorldPosition;

            if (worldPosition != null)
            {
                _startPoint = _points.OrderBy(v => v.Distance(worldPosition)).First();

                _index = _points.IndexOf(_startPoint);

                _startOffsetToVertex = worldPosition - _startPoint;

                _startGeometry = FeatureSource.Geometry!.Copy();

                _isEditing = true;
            }
        }

        private static bool IsRectangle(IList<MPoint> points)
        {
            if (points.Count != 5)
            {
                return false;
            }

            return IsOrthogonal(points[0], points[1], points[2]) &&
                   IsOrthogonal(points[1], points[2], points[3]) &&
                   IsOrthogonal(points[2], points[3], points[0]);

            static bool IsOrthogonal(MPoint a, MPoint b, MPoint c)
            {
                return Math.Abs((b.X - a.X) * (b.X - c.X) + (b.Y - a.Y) * (b.Y - c.Y)) < 1E-6;// == 0.0;
            }
        }
    }
}
