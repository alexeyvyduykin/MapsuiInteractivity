using Mapsui.Extensions;
using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI;

internal class DrawingManipulator : MouseManipulator
{
    public DrawingManipulator(IView view) : base(view) { }

    private bool _skip;
    private int _counter;
    private const int _minPixelsMovedForDrag = 4;

    public override void Completed(MouseEventArgs e)
    {
        base.Completed(e);

        if (_skip == true)
        {
            View.SetCursor(CursorType.Cross);
        }

        if (_skip == false)
        {
            var screenPosition = e.MapInfo?.ScreenPosition;

            bool isClick(MPoint worldPosition)
            {
                var p0 = View.Navigator.Viewport.WorldToScreen(worldPosition);

                var res = IsClick(p0, screenPosition);

                if (res == true)
                {
                    View.SetCursor(CursorType.Default);
                }

                return res;
            }

            View.Interactive.Ending(e.MapInfo, isClick);
        }

        e.Handled = true;
    }

    public override void Delta(MouseEventArgs e)
    {
        base.Delta(e);

        View.Interactive.Moving(e.MapInfo);

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

        View.Interactive.Starting(e.MapInfo);

        e.Handled = true;
    }

    private static bool IsClick(MPoint? screenPosition, MPoint? mouseDownScreenPosition)
    {
        if (mouseDownScreenPosition == null || screenPosition == null)
        {
            return false;
        }

        return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
    }
}

internal class HoverDrawingManipulator : MouseManipulator
{
    public HoverDrawingManipulator(IView view) : base(view)
    {

    }

    public override void Delta(MouseEventArgs e)
    {
        base.Delta(e);

        View.Interactive.Hovering(e.MapInfo);

        //e.Handled = true;
    }

    public override void Started(MouseEventArgs e)
    {
        base.Started(e);

        View.SetCursor(CursorType.Cross);

        e.Handled = true;
    }
}