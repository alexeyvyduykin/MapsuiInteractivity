using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Input;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia
{
    public partial class InteractivityBehavior
    {
        public IAvaloniaObject? AssociatedObject { get; private set; }

        public void Attach(IAvaloniaObject? associatedObject)
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
                mapControl.PointerLeave += MapControl_PointerLeave;
                mapControl.PointerEnter += MapControl_PointerEnter;
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
                mapControl.PointerLeave -= MapControl_PointerLeave;
                mapControl.PointerEnter -= MapControl_PointerEnter;
            }
        }

        private void MapControl_PointerEnter(object? sender, PointerEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (sender is MapControl mapControl && _behavior is not null)
            {
                var args = e.ToMouseEventArgs(mapControl);

                _controller?.HandleMouseEnter(new MapControlAdaptor(mapControl, _behavior), args);
            }
        }

        private void MapControl_PointerLeave(object? sender, PointerEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (sender is MapControl mapControl && _behavior is not null)
            {
                var args = e.ToMouseEventArgs(mapControl);

                _controller?.HandleMouseLeave(new MapControlAdaptor(mapControl, _behavior), args);
            }
        }

        private void MapControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (sender is MapControl mapControl && _behavior is not null)
            {
                var args = e.ToMouseWheelEventArgs(mapControl);

                _controller?.HandleMouseWheel(new MapControlAdaptor(mapControl, _behavior), args);
            }
        }

        private void MapControl_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (sender is MapControl mapControl && _behavior is not null)
            {
                mapControl.Focus();
                e.Pointer.Capture(mapControl);

                var args = e.ToMouseDownEventArgs(mapControl);

                _controller?.HandleMouseDown(new MapControlAdaptor(mapControl, _behavior), args);
            }
        }

        private void MapControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (e.Handled == true)
            {
                return;
            }

            if (sender is MapControl mapControl && _behavior is not null)
            {
                var args = e.ToMouseEventArgs(mapControl);

                _controller?.HandleMouseMove(new MapControlAdaptor(mapControl, _behavior), args);

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

            if (sender is MapControl mapControl && _behavior is not null)
            {
                var args = e.ToMouseReleasedEventArgs(mapControl);

                _controller?.HandleMouseUp(new MapControlAdaptor(mapControl, _behavior), args);
            }
        }
    }
}
