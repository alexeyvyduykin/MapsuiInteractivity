using Avalonia;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia;

public partial class InteractivityBehavior : AvaloniaObject
{
    private IController? _controller;
    private IInteractive? _interactive;

    static InteractivityBehavior()
    {
        InteractiveProperty.Changed.Subscribe(InteractiveChanged);
        StateProperty.Changed.Subscribe(StateChanged);
        MapProperty.Changed.Subscribe(MapChanged);
    }

    public static readonly StyledProperty<IInteractive?> InteractiveProperty =
        AvaloniaProperty.Register<InteractivityBehavior, IInteractive?>(nameof(Interactive));

    public IInteractive? Interactive
    {
        get => GetValue(InteractiveProperty);
        set => SetValue(InteractiveProperty, value);
    }

    public static readonly StyledProperty<string> StateProperty =
        AvaloniaProperty.Register<InteractivityBehavior, string>(nameof(State), States.Default);

    public string State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly StyledProperty<Map?> MapProperty =
        AvaloniaProperty.Register<InteractivityBehavior, Map?>(nameof(Map));

    public Map? Map
    {
        get => GetValue(MapProperty);
        set => SetValue(MapProperty, value);
    }

    private static void InteractiveChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is InteractivityBehavior behavior)
        {
            var value = e.GetNewValue<IInteractive>();

            behavior.SetInteractive(value);
        }
    }

    private static void StateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is InteractivityBehavior behavior)
        {
            var value = e.GetNewValue<string>();

            if (string.IsNullOrEmpty(value) == false)
            {
                behavior.SetController(value);
            }
        }
    }

    private static void MapChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is InteractivityBehavior behavior)
        {
            var value = e.GetNewValue<Map>();

            behavior.SetMap(value);
        }
    }

    private void SetController(string key)
    {
        _controller = InteractiveControllerFactory.GetController(key);

        if (AssociatedObject is MapControl mapControl && _interactive is not null)
        {
            // HACK: after tools check, hover manipulator not active, it call this          
            _controller.HandleMouseEnter(new MapControlAdaptor(mapControl, _interactive), new Input.Core.MouseEventArgs());
        }
    }

    private void SetInteractive(IInteractive interactive)
    {
        _interactive = interactive;
    }

    private void SetMap(Map? map)
    {
        if (AssociatedObject is MapControl mapControl && map is { })
        {
            mapControl.Map = map;
        }
    }
}
