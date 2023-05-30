using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI;

internal class EditingManipulator : MouseManipulator
{
    private bool _skip;
    private int _counter;
    private bool _isEditing = false;
    private readonly int _vertexRadius = 4;
    private IFeature? _clickFeature;
    private MPoint? _clickPoint;

    public EditingManipulator(IView view) : base(view) { }

    public override void Completed(MouseEventArgs e)
    {
        base.Completed(e);

        if (_isEditing == true)
        {
            View.Interactive.Ending(e.MapInfo);

            View.Navigator.PanLock = false;

            _isEditing = false;
        }
        else
        {
            var clickPoint = e.MapInfo?.WorldPosition;
            var clickFeature = e.MapInfo?.Feature;

            if (_skip == false
                && IsClick(clickPoint, _clickPoint) == true
                && IFeature.Equals(_clickFeature, clickFeature) == true)
            {
                View.Interactive.Cancel();
            }
        }

        View.SetCursor(CursorType.Default);

        e.Handled = true;
    }

    private static bool IsClick(MPoint? currentPosition, MPoint? previousPosition)
    {
        if (currentPosition == null || previousPosition == null)
            return false;

        return
            Math.Abs(currentPosition.X - previousPosition.X) < 1 &&
            Math.Abs(currentPosition.Y - previousPosition.Y) < 1;
    }

    public override void Delta(MouseEventArgs e)
    {
        base.Delta(e);

        if (_counter++ > 0)
        {
            _skip = true;
        }

        if (_isEditing == true)
        {
            View.Interactive.Moving(e.MapInfo);

            View.SetCursor(CursorType.HandGrab);

            e.Handled = true;
        }

        //e.Handled = true;
    }

    public override void Started(MouseEventArgs e)
    {
        base.Started(e);

        var mapInfo = e.MapInfo;

        _isEditing = false;

        _skip = false;
        _counter = 0;

        if (mapInfo?.IsInteractiveLayer() == true)
        {
            var distance = mapInfo.Resolution * _vertexRadius;

            View.Interactive.Starting(mapInfo, distance);

            _isEditing = true;
        }

        if (mapInfo != null && mapInfo.Feature != null)
        {
            _clickPoint = mapInfo.WorldPosition;
            _clickFeature = mapInfo.Feature;
        }

        if (_isEditing == true)
        {
            View.Navigator.PanLock = true;
        }

        e.Handled = true;
    }

}

internal class HoverEditingManipulator : MouseManipulator
{
    private bool _isChecker = false;

    public HoverEditingManipulator(IView view) : base(view) { }

    public override void Delta(MouseEventArgs e)
    {
        base.Delta(e);

        if (e.Handled == false)
        {
            var mapInfo = e.MapInfo;

            if (mapInfo?.IsInteractiveLayer() == true)
            {
                if (_isChecker == true)
                {
                    View.SetCursor(CursorType.Hand);

                    _isChecker = false;
                }

                e.Handled = true;
            }
            else
            {
                if (_isChecker == false)
                {
                    View.SetCursor(CursorType.Default);

                    _isChecker = true;
                }
            }
        }
    }
}
