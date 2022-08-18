using Mapsui;
using Mapsui.Nts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override void Starting(MPoint worldPosition)
        {
            _skip = false;
            _counter = 0;
        }

        public override void Moving(MPoint worldPosition)
        {
            if (_counter++ > 0)
            {
                _skip = true;
            }
        }

        public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            if (_skip == false)
            {
                CreatingFeature(worldPosition);
            }
        }

        public override void Hovering(MPoint worldPosition)
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
