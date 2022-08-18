using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class EditingManipulator : MouseManipulator
    {
        private bool _isEditing = false;
        private readonly int _vertexRadius = 4;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_isEditing == true)
            {
                var worldPosition = MapView.ScreenToWorld(e.Position);

                MapView.Behavior.OnCompleted(worldPosition);

                MapView.Map!.PanLock = false;

                _isEditing = false;
            }

            MapView.SetCursor(CursorType.Default);

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (_isEditing == true)
            {
                var worldPosition = MapView.ScreenToWorld(e.Position);

                MapView.Behavior.OnDelta(worldPosition);

                MapView.SetCursor(CursorType.HandGrab);

                e.Handled = true;
            }

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            var mapInfo = MapView.GetMapInfo(e.Position)!;

            _isEditing = false;

            if (mapInfo.Feature != null && mapInfo.Layer is InteractiveLayer)
            {
                var distance = mapInfo.Resolution * _vertexRadius;

                MapView.Behavior.OnStarted(mapInfo.WorldPosition!, distance);

                _isEditing = true;
            }

            if (_isEditing == true)
            {
                MapView.Map!.PanLock = true;
            }

            e.Handled = true;
        }

    }

    internal class HoverEditingManipulator : MouseManipulator
    {
        private bool _isChecker = false;

        public HoverEditingManipulator(IMapView view) : base(view) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (e.Handled == false)
            {
                var mapInfo = MapView.GetMapInfo(e.Position)!;

                if (mapInfo.Layer != null && mapInfo.Layer is InteractiveLayer)
                {
                    if (_isChecker == true)
                    {
                        MapView.SetCursor(CursorType.Hand);

                        _isChecker = false;
                    }

                    e.Handled = true;
                }
                else
                {
                    if (_isChecker == false)
                    {
                        MapView.SetCursor(CursorType.Default);

                        _isChecker = true;
                    }
                }
            }
        }
    }
}
