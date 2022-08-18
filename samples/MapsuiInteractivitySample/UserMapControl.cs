using Avalonia;
using Avalonia.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.UI.Avalonia;
using Mapsui.Projections;
using System;

namespace MapsuiInteractivitySample
{
    public class UserMapControl : InteractiveMapControl
    {
        private bool _isGrabbing = false;

        public UserMapControl() : base()
        {
            MapSourceProperty.Changed.Subscribe(OnMapSourceChanged);

            Navigator.CenterOn(SphericalMercator.FromLonLat(13, 42).ToMPoint());
            Navigator.ZoomTo(1000);
        }

        public Map MapSource
        {
            get { return (Map)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapSource.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<Map> MapSourceProperty =
            AvaloniaProperty.Register<UserMapControl, Map>(nameof(MapSource));

        private static void OnMapSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is Map map)
                {
                    mapControl.Map = map;
                }
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e.Handled == false)
            {
                var isLeftMouseDown = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;

                if (isLeftMouseDown == true)
                {
                    if (_isGrabbing == false)
                    {
                        _isGrabbing = true;

                        SetCursor(CursorType.HandGrab);
                    }
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (_isGrabbing == true)
            {
                _isGrabbing = false;

                if (e.Handled == false)
                {
                    SetCursor(CursorType.Default);
                }
            }

            base.OnPointerReleased(e);
        }
    }
}
