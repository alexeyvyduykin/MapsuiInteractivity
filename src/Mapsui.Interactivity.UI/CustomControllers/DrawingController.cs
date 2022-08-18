using Mapsui.Interactivity.UI.Input;

namespace Mapsui.Interactivity.UI
{
    public class DrawingController : ControllerBase, IMapController
    {
        public DrawingController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Drawing);
            this.BindMouseEnter(MapCommands.HoverDrawing);
        }
    }
}
