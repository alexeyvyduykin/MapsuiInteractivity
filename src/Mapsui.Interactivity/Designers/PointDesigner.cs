using Mapsui.Nts.Extensions;
using Mapsui.UI;

namespace Mapsui.Interactivity
{
    internal class PointDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;

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
                CreatingFeature(mapInfo?.WorldPosition!);
            }
        }

        public override void Hovering(MapInfo? mapInfo)
        {

        }

        public void CreatingFeature(MPoint worldPosition)
        {
            EndDrawing(worldPosition);

            EndCreatingCallback();

            return;
        }

        public void EndDrawing(MPoint worldPosition)
        {
            Feature = worldPosition.ToPoint().ToFeature();
        }
    }
}
