using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    public class SelectingController : BaseController, IController
    {
        public SelectingController()
        {
            var click = new DelegateViewCommand<MouseDownEventArgs>(
                (view, controller, args) => controller.AddMouseManipulator(view, new ClickManipulator(view), args));

            var hover = new DelegateViewCommand<MouseEventArgs>(
                (view, controller, args) => controller.AddHoverManipulator(view, new PointeroverManipulator(view), args));

            this.BindMouseDown(MouseButton.Left, click);
            this.BindMouseEnter(hover);
        }
    }
}
