using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal class PointeroverManipulator : MouseManipulator
    {
        private bool _isChecker = false;
        //private readonly IInteractiveBehavior _behavior;

        public PointeroverManipulator(IView view/*, IInteractiveBehavior behavior*/) : base(view)
        {
            //_behavior = behavior;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (e.Handled == false)
            {
                var mapInfo = e.MapInfo;

                if (mapInfo != null && mapInfo.Layer != null)
                {
                    if (_isChecker == true)
                    {
                        View.SetCursor(CursorType.Hand);

                        View.Behavior.OnHoverStart(mapInfo);

                        _isChecker = false;
                    }

                    e.Handled = false;// true;
                }
                else
                {
                    if (_isChecker == false)
                    {
                        View.SetCursor(CursorType.Default);

                        View.Behavior.OnHoverStop(mapInfo);

                        _isChecker = true;
                    }
                }
            }
        }
    }
}
