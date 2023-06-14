using Avalonia;
using Avalonia.Input;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;
using System.Diagnostics;
using System.Globalization;

namespace Mapsui.Interactivity.UI.Avalonia;

public partial class InteractivityBehavior
{
    public AvaloniaObject? AssociatedObject { get; private set; }

    public void Attach(AvaloniaObject? associatedObject)
    {
        if (Equals(associatedObject, AssociatedObject))
        {
            return;
        }

        if (AssociatedObject is { })
        {
            throw new InvalidOperationException(string.Format(
                CultureInfo.CurrentCulture,
                "An instance of a behavior cannot be attached to more than one object at a time."));
        }

        Debug.Assert(associatedObject is { }, "Cannot attach the behavior to a null object.");

        AssociatedObject = associatedObject ?? throw new ArgumentNullException(nameof(associatedObject));

        OnAttached();
    }

    public void Detach()
    {
        OnDetaching();
        AssociatedObject = null;
    }

    protected void OnAttached()
    {
        if (AssociatedObject is MapControl mapControl)
        {
            mapControl.PointerReleased += MapControl_PointerReleased;
            mapControl.PointerMoved += MapControl_PointerMoved;
            mapControl.PointerPressed += MapControl_PointerPressed;
            mapControl.PointerWheelChanged += MapControl_PointerWheelChanged;
            mapControl.PointerExited += MapControl_PointerExited;
            mapControl.PointerEntered += MapControl_PointerEntered;
        }
    }

    protected void OnDetaching()
    {
        if (AssociatedObject is MapControl mapControl)
        {
            mapControl.PointerReleased -= MapControl_PointerReleased;
            mapControl.PointerMoved -= MapControl_PointerMoved;
            mapControl.PointerPressed -= MapControl_PointerPressed;
            mapControl.PointerWheelChanged -= MapControl_PointerWheelChanged;
            mapControl.PointerExited -= MapControl_PointerExited;
            mapControl.PointerEntered -= MapControl_PointerEntered;
        }
    }

    private void MapControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            // var args = e.ToMouseEventArgs(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };

            _controller?.HandleMouseEnter(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_PointerExited(object? sender, PointerEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            //var args = e.ToMouseEventArgs(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };


            _controller?.HandleMouseLeave(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            var args = e.ToMouseWheelEventArgs(mapControl);

            _controller?.HandleMouseWheel(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            mapControl.Focus();
            e.Pointer.Capture(mapControl);

            //var args = e.ToMouseDownEventArgs(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseDownEventArgs
            {
#pragma warning disable CS0618 // Тип или член устарел
                // ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
                ChangedButton = e.GetCurrentPoint(null).Properties.PointerUpdateKind.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
                ClickCount = e.ClickCount,
                MapInfo = mapInfo
            };

            _controller?.HandleMouseDown(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Handled == true)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            //var args = e.ToMouseEventArgs(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };

            _controller?.HandleMouseMove(new MapControlAdaptor(mapControl, _interactive), args);

            e.Handled = args.Handled;
        }
    }

    private void MapControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        e.Pointer.Capture(null);

        if (sender is MapControl mapControl && _interactive is not null)
        {
            //var args = e.ToMouseReleasedEventArgs(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };

            _controller?.HandleMouseUp(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }
}
