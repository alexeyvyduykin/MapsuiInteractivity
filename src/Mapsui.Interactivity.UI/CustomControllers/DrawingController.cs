using Mapsui.Interactivity.UI.Input;

namespace Mapsui.Interactivity.UI;

public class DrawingController : BaseController, IController
{
    public DrawingController()
    {
        this.BindMouseDown(MouseButton.Left, MapCommands.Drawing);
        this.BindMouseEnter(MapCommands.HoverDrawing);
    }
}
