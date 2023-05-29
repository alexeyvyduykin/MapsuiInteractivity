using Avalonia;
using Avalonia.Input;
using Mapsui.Extensions;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia;

public class InteractivityMapView : MapControl, IView
{
    private IController? _controller;
    private IInteractive? _interactive;

    public static readonly StyledProperty<Map?> MapSourceProperty =
        AvaloniaProperty.Register<InteractivityMapView, Map?>(nameof(MapSource));

    public static readonly StyledProperty<IInteractive?> InteractiveProperty =
        AvaloniaProperty.Register<InteractivityMapView, IInteractive?>(nameof(Interactive));

    public static readonly StyledProperty<string> StateProperty =
        AvaloniaProperty.Register<InteractivityMapView, string>(nameof(State), States.Default);

    public Map? MapSource
    {
        get { return GetValue(MapSourceProperty); }
        set { SetValue(MapSourceProperty, value); }
    }

    public IInteractive? Interactive
    {
        get { return GetValue(InteractiveProperty); }
        set { SetValue(InteractiveProperty, value); }
    }

    public string State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MapSourceProperty)
        {
            if (change.NewValue.GetValueOrDefault() is Map map)
            {
                Map = map;
            }
        }
        else if (change.Property == InteractiveProperty)
        {
            if (change.NewValue.GetValueOrDefault() is IInteractive interactive)
            {
                _interactive = interactive;
            }
        }
        else if (change.Property == StateProperty)
        {
            if (change.NewValue.GetValueOrDefault() is string state && string.IsNullOrEmpty(state) == false)
            {
                _controller = InteractiveControllerFactory.GetController(state);

                //if (_interactive is not null)
                {
                    // HACK: after tools check, hover manipulator not active, it call this          
                    _controller.HandleMouseEnter(this/*new MapControlAdaptor(this, _interactive)*/, new Input.Core.MouseEventArgs());
                }
            }
        }
    }

    //public Map? Map => _mapControl.Map;

    // public IInteractive Interactive => _interactive;

    public void SetCursor(CursorType cursorType)
    {
        Cursor = new Cursor(cursorType.ToStandartCursor());
    }

    public MPoint WorldToScreen(MPoint worldPosition)
    {
        return Map.Navigator.Viewport.WorldToScreen(worldPosition);
    }

    protected override void OnPointerEnter(PointerEventArgs e)
    {
        base.OnPointerEnter(e);

        if (e.Handled)
        {
            return;
        }

        _controller?.HandleMouseEnter(this, e.ToMouseEventArgs(this));
    }

    protected override void OnPointerLeave(PointerEventArgs e)
    {
        base.OnPointerLeave(e);

        if (e.Handled)
        {
            return;
        }

        _controller?.HandleMouseLeave(this, e.ToMouseEventArgs(this));
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (e.Handled)
        {
            return;
        }

        _controller?.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Handled)
        {
            return;
        }

        this.Focus();

        e.Pointer.Capture(this);

        _controller?.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (e.Handled == true)
        {
            return;
        }

        try
        {
            var args = e.ToMouseEventArgs(this);

            _controller?.HandleMouseMove(this, args);

            // TODO: ?
            e.Handled = args.Handled;
        }
        catch (Exception)
        {
            return;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Handled)
        {
            return;
        }

        e.Pointer.Capture(null);

        _controller?.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));
    }
}
