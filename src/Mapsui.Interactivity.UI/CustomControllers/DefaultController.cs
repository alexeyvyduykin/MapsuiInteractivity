using Mapsui.Interactivity.UI.Input;

namespace Mapsui.Interactivity.UI
{
    public class DefaultController : BaseController, IController
    {
        public DefaultController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Default);
            this.BindMouseEnter(MapCommands.HoverDefault);
        }
    }
}
