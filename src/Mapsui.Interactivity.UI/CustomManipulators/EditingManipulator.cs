using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class EditingManipulator : MouseManipulator
    {
        private bool _isEditing = false;
        private readonly int _vertexRadius = 4;

        public EditingManipulator(IView view) : base(view) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_isEditing == true)
            {
                //var worldPosition = e.MapInfo?.WorldPosition;// View.ScreenToWorld(e.Position);

                View.Behavior.OnCompleted(e.MapInfo);

                View.Map!.PanLock = false;

                _isEditing = false;
            }

            View.SetCursor(CursorType.Default);

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (_isEditing == true)
            {
                var worldPosition = View.ScreenToWorld(e.Position);

                View.Behavior.OnDelta(worldPosition);

                View.SetCursor(CursorType.HandGrab);

                e.Handled = true;
            }

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            var mapInfo = e.MapInfo;

            _isEditing = false;

            if (mapInfo != null && mapInfo.Feature != null && mapInfo.Layer is InteractiveLayer)
            {
                var distance = mapInfo.Resolution * _vertexRadius;

                View.Behavior.OnStarted(mapInfo.WorldPosition!, distance);

                _isEditing = true;
            }

            if (_isEditing == true)
            {
                View.Map!.PanLock = true;
            }

            e.Handled = true;
        }

    }

    internal class HoverEditingManipulator : MouseManipulator
    {
        private bool _isChecker = false;

        public HoverEditingManipulator(IView view) : base(view) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (e.Handled == false)
            {
                var mapInfo = e.MapInfo;

                if (mapInfo != null && mapInfo.Layer != null && mapInfo.Layer is InteractiveLayer)
                {
                    if (_isChecker == true)
                    {
                        View.SetCursor(CursorType.Hand);

                        _isChecker = false;
                    }

                    e.Handled = true;
                }
                else
                {
                    if (_isChecker == false)
                    {
                        View.SetCursor(CursorType.Default);

                        _isChecker = true;
                    }
                }
            }
        }
    }
}
