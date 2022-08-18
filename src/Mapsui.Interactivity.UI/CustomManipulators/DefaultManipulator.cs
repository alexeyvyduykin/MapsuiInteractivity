using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class DefaultManipulator : MouseManipulator
    {
        public DefaultManipulator(IMapView mapView) : base(mapView)
        {
            MapView.SetCursor(CursorType.Default);
        }
    }

    internal class HoverDefaultManipulator : MouseManipulator
    {
        public HoverDefaultManipulator(IMapView view) : base(view)
        {

        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            MapView.SetCursor(CursorType.Default);

            e.Handled = true;
        }
    }
}
