using Avalonia;
using Avalonia.Input;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia
{
    public class InteractiveMapControl : MapControl, IView
    {
        public InteractiveMapControl() : base()
        {
            ControllerProperty.Changed.Subscribe(OnControllerChanged);
        }

        public IController Controller
        {
            get { return GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IController> ControllerProperty =
            AvaloniaProperty.Register<InteractiveMapControl, IController>(nameof(Controller), new DefaultController());

        private static void OnControllerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (InteractiveMapControl)e.Sender;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new Input.Core.MouseEventArgs());
        }

        public IInteractiveBehavior Behavior
        {
            get { return GetValue(BehaviorProperty); }
            set { SetValue(BehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IInteractiveBehavior> BehaviorProperty =
            AvaloniaProperty.Register<InteractiveMapControl, IInteractiveBehavior>(nameof(Behavior));

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.Handled)
            {
                return;
            }

            e.Pointer.Capture(null);

            var args = e.ToMouseReleasedEventArgs(this);

            Controller.HandleMouseUp(this, args);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e.Handled == true)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseMove(this, args);

            e.Handled = args.Handled;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.Handled)
            {
                return;
            }

            Focus();
            e.Pointer.Capture(this);

            var args = e.ToMouseDownEventArgs(this);

            Controller.HandleMouseDown(this, args);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseWheelEventArgs(this);

            Controller.HandleMouseWheel(this, args);
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseLeave(this, args);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseEnter(this, args);
        }

        public virtual void SetCursor(CursorType cursorType)
        {
            Cursor = new Cursor(cursorType.ToStandartCursor());
        }

        public MPoint ScreenToWorld(MPoint screenPosition) => Viewport.ScreenToWorld(screenPosition);

        public MPoint WorldToScreen(MPoint worldPosition) => Viewport.WorldToScreen(worldPosition);
    }
}
