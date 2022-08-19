﻿using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class DrawingManipulator : MouseManipulator
    {
        public DrawingManipulator(IView view) : base(view) { }

        private bool _skip;
        private int _counter;
        private const int _minPixelsMovedForDrag = 4;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == true)
            {
                View.SetCursor(CursorType.Cross);
            }

            if (_skip == false)
            {
                var screenPosition = e.Position;

                bool isClick(MPoint worldPosition)
                {
                    var p0 = View.WorldToScreen(worldPosition);

                    var res = IsClick(p0, screenPosition);

                    if (res == true)
                    {
                        View.SetCursor(CursorType.Default);
                    }

                    return res;
                }

                View.Behavior.OnCompleted(e.MapInfo, isClick);
            }

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = View.ScreenToWorld(screenPosition);

            View.Behavior.OnDelta(worldPosition);

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
            var worldPosition = View.ScreenToWorld(screenPosition);

            View.Behavior.OnStarted(worldPosition, 0);

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
        public HoverDrawingManipulator(IView view) : base(view)
        {

        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            View.Behavior.OnHover(e.MapInfo);

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            View.SetCursor(CursorType.Cross);

            e.Handled = true;
        }
    }
}