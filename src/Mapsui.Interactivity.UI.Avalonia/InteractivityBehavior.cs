using Avalonia;
using Avalonia.Data;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia;

public partial class InteractivityBehavior : AvaloniaObject
{
    private IController? _controller;
    private IInteractive? _interactive;

    static InteractivityBehavior()
    {
        InteractiveProperty.Changed.Subscribe(s => HandleInteractiveChanged(s.Sender, s.NewValue.GetValueOrDefault<IInteractive>()));
        StateProperty.Changed.Subscribe(s => HandleStateChanged(s.Sender, s.NewValue.GetValueOrDefault<string>()));
        MapProperty.Changed.Subscribe(s => HandleMapChanged(s.Sender, s.NewValue.GetValueOrDefault<Map>()));
    }

    // HACK: [Binding] Error in binding to 'Mapsui.Interactivity.UI.Avalonia.InteractivityBehavior'.'Interactive': 'Null value in expression '{empty}' at ''.' (InteractivityBehavior #16468652)

    public static readonly StyledProperty<IInteractive?> InteractiveProperty =
        AvaloniaProperty.Register<InteractivityBehavior, IInteractive?>(nameof(Interactive), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public IInteractive? Interactive
    {
        get { return GetValue(InteractiveProperty); }
        set { SetValue(InteractiveProperty, value); }
    }

    public static readonly StyledProperty<string> StateProperty =
        AvaloniaProperty.Register<InteractivityBehavior, string>(nameof(State), States.Default, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : string.Empty);

    public string State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    public static readonly StyledProperty<Map?> MapProperty =
        AvaloniaProperty.Register<InteractivityBehavior, Map?>(nameof(Map), null, false, BindingMode.OneWay, coerce: (s, value) => value is { } ? value : null);

    public Map? Map
    {
        get { return GetValue(MapProperty); }
        set { SetValue(MapProperty, value); }
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
        if (AssociatedObject is MapControl mapControl)
        {
            mapControl.Map = map;
        }
    }

    private static void HandleInteractiveChanged(IAvaloniaObject element, IInteractive? value)
    {
        if (element is InteractivityBehavior behavior && value is not null)
        {
            behavior.SetInteractive(value);
        }
    }

    private static void HandleStateChanged(IAvaloniaObject element, string? value)
    {
        if (element is InteractivityBehavior behavior && string.IsNullOrEmpty(value) == false)
        {
            behavior.SetController(value);
        }
    }

    private static void HandleMapChanged(IAvaloniaObject element, Map? value)
    {
        if (element is InteractivityBehavior behavior)
        {
            behavior.SetMap(value);
        }
    }
}
