using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui;

namespace Mapsui.Interactivity.UI
{
    internal class DrawingManipulator : MouseManipulator
    {
        public DrawingManipulator(IMapView view) : base(view) { }

        private bool _skip;
        private int _counter;
        private const int _minPixelsMovedForDrag = 4;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == true)
            {
                MapView.SetCursor(CursorType.Cross);
            }

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.ScreenToWorld(screenPosition);

                bool isClick(MPoint worldPosition)
                {
                    var p0 = MapView.WorldToScreen(worldPosition);

                    var res = IsClick(p0, screenPosition);

                    if (res == true)
                    {
                        MapView.SetCursor(CursorType.Default);
                    }

                    return res;
                }

                MapView.Behavior.OnCompleted(worldPosition, isClick);
            }

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.Behavior.OnDelta(worldPosition);

            if (_counter++ > 0)
            {
                _skip = true;

                return;
            }

            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            _skip = false;
            _counter = 0;

            var screenPosition = e.Position;
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.Behavior.OnStarted(worldPosition, 0);

            e.Handled = true;
        }

        private static bool IsClick(MPoint screenPosition, MPoint mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }

    internal class HoverDrawingManipulator : MouseManipulator
    {
        public HoverDrawingManipulator(IMapView view) : base(view)
        {

        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.Behavior.OnHover(worldPosition);

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            MapView.SetCursor(CursorType.Cross);

            e.Handled = true;
        }
    }
}