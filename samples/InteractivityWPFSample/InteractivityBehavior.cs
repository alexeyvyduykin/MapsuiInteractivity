using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.UI.Wpf;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace InteractivityWPFSample;

public partial class InteractivityBehavior : Behavior<MapControl>
{
    private IController? _controller;
    private IInteractive? _interactive;

    public static readonly DependencyProperty InteractiveProperty =
        DependencyProperty.Register("Interactive", typeof(IInteractive), typeof(InteractivityBehavior), new PropertyMetadata(default(IInteractive), InteractivePropertyChanged));

    public IInteractive? Interactive
    {
        get { return (IInteractive?)GetValue(InteractiveProperty); }
        set { SetValue(InteractiveProperty, value); }
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(string), typeof(InteractivityBehavior), new PropertyMetadata(States.Default, StatePropertyChanged));

    public string State
    {
        get { return (string)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    public static readonly DependencyProperty MapProperty =
        DependencyProperty.Register("Map", typeof(Map), typeof(InteractivityBehavior), new PropertyMetadata(null, MapPropertyChanged));

    public Map? Map
    {
        get { return (Map?)GetValue(MapProperty); }
        set { SetValue(MapProperty, value); }
    }

    private static void MapPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is InteractivityBehavior behavior)
        {
            var value = (Map)e.NewValue;

            behavior.MapChanged(value);
        }
    }

    private void MapChanged(Map map)
    {
        if (_mapControl is { })
        {
            _mapControl.Map = map;
        }
    }

    private static void StatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is InteractivityBehavior behavior)
        {
            var value = (string)e.NewValue;

            if (string.IsNullOrEmpty(value) == false)
            {
                behavior.StateChanged(value);
            }
        }
    }

    private void StateChanged(string key)
    {
        _controller = InteractiveControllerFactory.GetController(key);

        if (_mapControl is { } && _interactive is not null)
        {
            // HACK: after tools check, hover manipulator not active, it call this          
            _controller.HandleMouseEnter(new MapControlAdaptor(_mapControl, _interactive), new Mapsui.Interactivity.UI.Input.Core.MouseEventArgs());
        }
    }

    private static void InteractivePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is InteractivityBehavior behavior)
        {
            var value = (IInteractive)e.NewValue;

            behavior.InteractiveChanged(value);
        }
    }

    private void InteractiveChanged(IInteractive interactive)
    {
        _interactive = interactive;
    }
}
