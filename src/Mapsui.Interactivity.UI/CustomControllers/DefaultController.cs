using Mapsui.Interactivity.UI.Input;

namespace Mapsui.Interactivity.UI
{
    public class DefaultController : ControllerBase, IMapController
    {
        public DefaultController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Default);
            this.BindMouseEnter(MapCommands.HoverDefault);
        }
    }
}
