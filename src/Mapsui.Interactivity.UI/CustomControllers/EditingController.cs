using Mapsui.Interactivity.UI.Input;

namespace Mapsui.Interactivity.UI
{
    public class EditingController : BaseController, IController
    {
        public EditingController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Editing);
            this.BindMouseEnter(MapCommands.HoverEditing);
        }
    }
}
