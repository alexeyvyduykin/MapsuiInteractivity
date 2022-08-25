using Avalonia.Input;
using Mapsui.Extensions;
using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.UI.Avalonia;
using Mapsui.Projections;

namespace MapsuiInteractivitySample
{
    public class UserMapControl : InteractiveMapControl
    {
        private bool _isGrabbing = false;

        public UserMapControl() : base()
        {
            Navigator.CenterOn(SphericalMercator.FromLonLat(13, 42).ToMPoint());
            Navigator.ZoomTo(1000);
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
