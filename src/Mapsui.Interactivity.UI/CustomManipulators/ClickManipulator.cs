using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class ClickManipulator : MouseManipulator
    {
        //private readonly IInteractiveBehavior _behavior;

        public ClickManipulator(IView view/*, IInteractiveBehavior behavior*/) : base(view)
        {
            //_behavior = behavior;
        }

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
