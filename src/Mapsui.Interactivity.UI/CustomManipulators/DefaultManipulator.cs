using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class DefaultManipulator : MouseManipulator
    {
        public DefaultManipulator(IView view) : base(view)
        {
            View.SetCursor(CursorType.Default);
        }
    }

    internal class HoverDefaultManipulator : MouseManipulator
    {
        public HoverDefaultManipulator(IView view) : base(view)
        {

        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            View.SetCursor(CursorType.Default);

            e.Handled = true;
        }
    }
}
