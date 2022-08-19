using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    public class CustomController : BaseController, IController
    {
        public CustomController(/*IInteractive interactive*/)
        {
            //var behavior = new InteractiveBehavior(interactive);

            var click = new DelegateViewCommand<MouseDownEventArgs>(
                (view, controller, args) => controller.AddMouseManipulator(view, new ClickManipulator(view/*, behavior*/), args));

            var hover = new DelegateViewCommand<MouseEventArgs>(
                (view, controller, args) => controller.AddHoverManipulator(view, new PointeroverManipulator(view/*, behavior*/), args));

            this.BindMouseDown(MouseButton.Left, click);
            this.BindMouseEnter(hover);
        }
    }
}
