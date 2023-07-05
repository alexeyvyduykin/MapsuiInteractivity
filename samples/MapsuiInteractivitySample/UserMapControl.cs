using Avalonia.Input;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.UI.Avalonia;

namespace MapsuiInteractivitySample;

public class UserMapControl : MapControl
{
    private bool _isGrabbing = false;
    private Cursor? _prevCursor = Cursor.Default;

    public UserMapControl() : base()
    {
        Map.Navigator.CenterOn(SphericalMercator.FromLonLat(13, 42).ToMPoint());
        Map.Navigator.ZoomTo(1000);        
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

                    _prevCursor = Cursor;

                    Cursor = new Cursor(StandardCursorType.SizeAll);
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
                Cursor = _prevCursor;
            }
        }

        base.OnPointerReleased(e);
    }
}
