using InteractivityWPFSample.Extensions;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Wpf;
using Mapsui.UI.Wpf.Extensions;

namespace InteractivityWPFSample;

public partial class InteractivityBehavior
{
    private MapControl? _mapControl;

    protected override void OnAttached()
    {
        base.OnAttached();

        _mapControl = this.AssociatedObject;

        _mapControl.MouseLeave += MapControl_MouseLeave;
        _mapControl.MouseEnter += MapControl_MouseEnter;
        _mapControl.MouseMove += MapControl_MouseMove;
        _mapControl.MouseDown += MapControl_MouseDown;
        _mapControl.MouseWheel += MapControl_MouseWheel;
        _mapControl.MouseUp += MapControl_MouseUp;

        if (Map is { })
        {
            _mapControl.Map = Map;
        }
    }


    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (_mapControl is { })
        {
            _mapControl.MouseLeave -= MapControl_MouseLeave;
            _mapControl.MouseEnter -= MapControl_MouseEnter;
            _mapControl.MouseMove -= MapControl_MouseMove;
            _mapControl.MouseDown -= MapControl_MouseDown;
            _mapControl.MouseWheel -= MapControl_MouseWheel;
            _mapControl.MouseUp -= MapControl_MouseUp;

            _mapControl = null;
        }
    }

    private void MapControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };


            _controller?.HandleMouseLeave(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.Handled == true)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
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
    private void MapControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            mapControl.Focus();
            e.MouseDevice.Capture(mapControl);

            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseDownEventArgs
            {
#pragma warning disable CS0618 // Тип или член устарел
                ChangedButton = e.ChangedButton.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
                ClickCount = e.ClickCount,
                MapInfo = mapInfo
            };

            _controller?.HandleMouseDown(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            var args = new MouseWheelEventArgs
            {
                Delta = (int)(e.Delta/*.Y*/ + e.Delta/*.X*/) * 120
            };

            _controller?.HandleMouseWheel(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }
    private void MapControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (sender is MapControl mapControl && _interactive is not null)
        {
            var position = e.GetPosition(mapControl).ToMapsui();

            var mapInfo = mapControl.GetMapInfo(position);

            var args = new MouseEventArgs
            {
                MapInfo = mapInfo,
            };

            _controller?.HandleMouseEnter(new MapControlAdaptor(mapControl, _interactive), args);
        }
    }

    private void MapControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        e.MouseDevice.Capture(null);

        if (sender is MapControl mapControl && _interactive is not null)
        {
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
