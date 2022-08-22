using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class EditingManipulator : MouseManipulator
    {
        private bool _isEditing = false;
        private readonly int _vertexRadius = 4;
        private IFeature? _clickFeature;
        private MPoint? _clickPoint;

        public EditingManipulator(IView view) : base(view) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_isEditing == true)
            {
                View.Behavior.OnCompleted(e.MapInfo);

                View.Map!.PanLock = false;

                _isEditing = false;
            }
            else
            {
                var clickPoint = e.MapInfo?.WorldPosition;
                var clickFeature = e.MapInfo?.Feature;

                if (IsClick(clickPoint, _clickPoint) == true
                    && IFeature.Equals(_clickFeature, clickFeature) == true)
                {
                    View.Behavior.OnCancel();
                }
            }

            View.SetCursor(CursorType.Default);

            e.Handled = true;
        }

        private static bool IsClick(MPoint? currentPosition, MPoint? previousPosition)
        {
            if (currentPosition == null || previousPosition == null)
                return false;

            return
                Math.Abs(currentPosition.X - previousPosition.X) < 1 &&
                Math.Abs(currentPosition.Y - previousPosition.Y) < 1;
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

            if (mapInfo != null && mapInfo.Feature != null)
            {
                _clickPoint = mapInfo.WorldPosition;
                _clickFeature = mapInfo.Feature;
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
