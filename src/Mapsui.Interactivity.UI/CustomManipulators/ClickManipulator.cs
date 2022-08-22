using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class ClickManipulator : MouseManipulator
    {
        public ClickManipulator(IView view) : base(view) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (e.Handled == false)
            {
                var mapInfo = e.MapInfo;

                if (mapInfo != null &&
                    mapInfo.Layer != null &&
                    mapInfo.Feature != null)
                {
                    View.Behavior.OnCompleted(mapInfo);
                }
            }

            e.Handled = true;
        }
    }
}
