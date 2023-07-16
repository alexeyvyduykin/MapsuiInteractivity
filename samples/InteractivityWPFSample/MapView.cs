using InteractivityWPFSample.Extensions;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Wpf;
using Mapsui.UI.Wpf.Extensions;
using System.Windows;

namespace InteractivityWPFSample;

public class InteractivityMapView : MapControl, IView
{
    private IController? _controller;
    private IInteractive? _interactive;


    public static readonly DependencyProperty MapSourceProperty =
        DependencyProperty.RegisterAttached("MapSource", typeof(Map), typeof(InteractivityMapView), new PropertyMetadata(default(Map)));

    public static void SetMapSource(UIElement element, Map? value)
    {
        element.SetValue(MapSourceProperty, value);
    }

    public static Map? GetMapSource(UIElement element)
    {
        return (Map?)element.GetValue(MapSourceProperty);
    }


    public static readonly DependencyProperty InteractiveSourceProperty =
        DependencyProperty.RegisterAttached("InteractiveSource", typeof(IInteractive), typeof(InteractivityMapView), new PropertyMetadata(default(IInteractive)));

    public static void SetInteractiveSource(UIElement element, IInteractive value)
    {
        element.SetValue(InteractiveSourceProperty, value);
    }

    public static IInteractive GetInteractiveSource(UIElement element)
    {
        return (IInteractive)element.GetValue(InteractiveSourceProperty);
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.RegisterAttached("State", typeof(string), typeof(InteractivityMapView), new PropertyMetadata(States.Default));

    public static void SetState(UIElement element, string value)
    {
        element.SetValue(StateProperty, value);
    }

    public static string GetState(UIElement element)
    {
        return (string)element.GetValue(StateProperty);
    }

    public Navigator Navigator => Map.Navigator;

    public IInteractive Interactive => _interactive;

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == MapSourceProperty)
        {
            Map = (Map)e.NewValue;
        }
        else if (e.Property == InteractiveSourceProperty)
        {
            _interactive = (IInteractive)e.NewValue;
        }
        else if (e.Property == StateProperty)
        {
            var state = (string)e.NewValue;

            if (string.IsNullOrEmpty(state) == false)
            {
                _controller = InteractiveControllerFactory.GetController(state);

                // HACK: after tools check, hover manipulator not active, it call this          
                _controller.HandleMouseEnter(this, new MouseEventArgs());
            }
        }
    }

    public virtual void SetCursor(Mapsui.Interactivity.UI.CursorType cursorType)
    {
        Cursor = cursorType.ToStandartCursor();
    }

    public MPoint WorldToScreen(MPoint worldPosition)
    {
        return Map.Navigator.Viewport.WorldToScreen(worldPosition);
    }

    protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        if (e.Handled)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseEnter(this, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        if (e.Handled)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseLeave(this, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (e.Handled)
        {
            return;
        }

        var args = new MouseWheelEventArgs
        {
            Delta = (int)(e.Delta/*.Y*/ + e.Delta/*.X*/) * 120
        };

        _controller?.HandleMouseWheel(this, args);
    }

    protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Handled)
        {
            return;
        }

        this.Focus();

        e.MouseDevice.Capture(this);

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        var args = new MouseDownEventArgs
        {
#pragma warning disable CS0618 // Тип или член устарел
            //ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
            // ChangedButton = e.GetCurrentPoint(null).Properties.PointerUpdateKind.Convert(),
            ChangedButton = e.ChangedButton.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
            ClickCount = e.ClickCount,
            MapInfo = mapInfo
        };

        _controller?.HandleMouseDown(this, args);
    }

    protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.Handled == true)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        var args = new MouseEventArgs
        {
            MapInfo = mapInfo,
        };

        _controller?.HandleMouseMove(this, args);

        // TODO: ?
        e.Handled = args.Handled;
    }

    protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Handled)
        {
            return;
        }

        e.MouseDevice.Capture(null);

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseUp(this, new MouseEventArgs { MapInfo = mapInfo });
    }
}
