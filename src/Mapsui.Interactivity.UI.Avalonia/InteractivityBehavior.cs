using Avalonia;
using Avalonia.Data;
using Mapsui.UI.Avalonia;
using System.Diagnostics;
using System.Globalization;
using mapsui = Mapsui.Interactivity;

namespace Mapsui.Interactivity.UI.Avalonia
{
    public class InteractivityBehavior : AvaloniaObject
    {
        #region MyRegion

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

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        internal void AttachedToVisualTree()
        {
            OnAttachedToVisualTree();
        }

        internal void DetachedFromVisualTree()
        {
            OnDetachedFromVisualTree();
        }

        protected virtual void OnAttachedToVisualTree()
        {
        }

        protected virtual void OnDetachedFromVisualTree()
        {
        }

        #endregion

        static InteractivityBehavior()
        {
            InteractiveProperty.Changed.Subscribe(s => HandleInteractiveChanged(s.Sender, s.NewValue.GetValueOrDefault<mapsui.IInteractive>()));
            StateProperty.Changed.Subscribe(s => HandleStateChanged(s.Sender, s.NewValue.GetValueOrDefault<string>()));
            MapProperty.Changed.Subscribe(s => HandleMapChanged(s.Sender, s.NewValue.GetValueOrDefault<Map>()));
        }

        public static readonly StyledProperty<mapsui.IInteractive?> InteractiveProperty =
            AvaloniaProperty.Register<InteractivityBehavior, mapsui.IInteractive?>(nameof(Interactive), null, false, BindingMode.OneWay);

        public mapsui.IInteractive? Interactive
        {
            get { return GetValue(InteractiveProperty); }
            set { SetValue(InteractiveProperty, value); }
        }

        public static readonly StyledProperty<string> StateProperty =
            AvaloniaProperty.Register<InteractivityBehavior, string>(nameof(State), States.Default, false, BindingMode.OneWay);

        public string State
        {
            get { return GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly StyledProperty<Map?> MapProperty =
            AvaloniaProperty.Register<InteractivityBehavior, Map?>(nameof(Map), null, false, BindingMode.OneWay);

        public Map? Map
        {
            get { return GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        private static void HandleInteractiveChanged(IAvaloniaObject element, mapsui.IInteractive? value)
        {
            if (element is InteractivityBehavior behavior && value is not null)
            {
                if (behavior.AssociatedObject is InteractiveMapControl mapControl)
                {
                    mapControl.Behavior = new InteractiveBehavior(value);
                }
            }
        }

        private static void HandleStateChanged(IAvaloniaObject element, string? value)
        {
            if (element is InteractivityBehavior behavior && string.IsNullOrEmpty(value) == false)
            {
                if (behavior.AssociatedObject is InteractiveMapControl mapControl)
                {
                    mapControl.Controller = InteractiveControllerFactory.GetController(value);
                }
            }
        }

        private static void HandleMapChanged(IAvaloniaObject element, Map? value)
        {
            if (element is InteractivityBehavior behavior)
            {
                if (behavior.AssociatedObject is MapControl mapControl)
                {
                    mapControl.Map = value;
                }
            }
        }
    }
}
