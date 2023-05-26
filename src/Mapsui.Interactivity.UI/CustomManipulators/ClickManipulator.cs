using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI;

internal class ClickManipulator : MouseManipulator
{
    private bool _skip;
    private int _counter;

    public ClickManipulator(IView view) : base(view) { }

    public override void Completed(MouseEventArgs e)
    {
        base.Completed(e);

        if (_skip == false && e.Handled == false)
        {
            var mapInfo = e.MapInfo;

            if (mapInfo != null &&
                mapInfo.Layer != null &&
                mapInfo.Feature != null)
            {
                View.Interactive.Ending(mapInfo);
            }
        }

        e.Handled = true;
    }

    public override void Delta(MouseEventArgs e)
    {
        base.Delta(e);

        if (_counter++ > 0)
        {
            _skip = true;

            return;
        }

        e.Handled = true;
    }

    public override void Started(MouseEventArgs e)
    {
        base.Started(e);

        _skip = false;
        _counter = 0;

        e.Handled = true;
    }
}
