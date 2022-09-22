using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

namespace MapsuiInteractivitySample
{
    public interface IBehavior
    {
        IAvaloniaObject? AssociatedObject { get; }

        void Attach(IAvaloniaObject? associatedObject);

        void Detach();
    }

    public class InteractionTest
    {
        static InteractionTest()
        {
            BehaviorsProperty.Changed.Subscribe(BehaviorsChanged);
        }

        public static readonly AttachedProperty<BehaviorCollectionTest?> BehaviorsProperty =
            AvaloniaProperty.RegisterAttached<InteractionTest, IAvaloniaObject, BehaviorCollectionTest?>("Behaviors");

        public static BehaviorCollectionTest GetBehaviors(IAvaloniaObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var behaviorCollection = (BehaviorCollectionTest?)obj.GetValue(BehaviorsProperty);
            if (behaviorCollection is null)
            {
                behaviorCollection = new BehaviorCollectionTest();
                obj.SetValue(BehaviorsProperty, behaviorCollection);
                SetVisualTreeEventHandlersInitial(obj);
            }

            return behaviorCollection;
        }

        public static void SetBehaviors(IAvaloniaObject obj, BehaviorCollectionTest? value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            obj.SetValue(BehaviorsProperty, value);
        }

        private static void BehaviorsChanged(AvaloniaPropertyChangedEventArgs<BehaviorCollectionTest?> e)
        {
            var oldCollection = e.OldValue.GetValueOrDefault();
            var newCollection = e.NewValue.GetValueOrDefault();

            if (oldCollection == newCollection)
            {
                return;
            }

            if (oldCollection is { AssociatedObject: { } })
            {
                oldCollection.Detach();
            }

            if (newCollection is { })
            {
                newCollection.Attach(e.Sender);
                SetVisualTreeEventHandlersRuntime(e.Sender);
            }
        }

        private static void SetVisualTreeEventHandlersInitial(IAvaloniaObject obj)
        {
            if (obj is not Control control)
            {
                return;
            }

            control.AttachedToVisualTree -= Control_AttachedToVisualTreeRuntime;
            control.DetachedFromVisualTree -= Control_DetachedFromVisualTreeRuntime;

            control.AttachedToVisualTree -= Control_AttachedToVisualTreeInitial;
            control.AttachedToVisualTree += Control_AttachedToVisualTreeInitial;

            control.DetachedFromVisualTree -= Control_DetachedFromVisualTreeInitial;
            control.DetachedFromVisualTree += Control_DetachedFromVisualTreeInitial;
        }

        private static void SetVisualTreeEventHandlersRuntime(IAvaloniaObject obj)
        {
            if (obj is not Control control)
            {
                return;
            }

            control.AttachedToVisualTree -= Control_AttachedToVisualTreeInitial;
            control.DetachedFromVisualTree -= Control_DetachedFromVisualTreeInitial;

            control.AttachedToVisualTree -= Control_AttachedToVisualTreeRuntime;
            control.AttachedToVisualTree += Control_AttachedToVisualTreeRuntime;

            control.DetachedFromVisualTree -= Control_DetachedFromVisualTreeRuntime;
            control.DetachedFromVisualTree += Control_DetachedFromVisualTreeRuntime;
        }

        private static void Control_AttachedToVisualTreeInitial(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d).Attach(d);
                GetBehaviors(d).AttachedToVisualTree();
            }
        }

        private static void Control_DetachedFromVisualTreeInitial(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d).DetachedFromVisualTree();
                GetBehaviors(d).Detach();
            }
        }

        private static void Control_AttachedToVisualTreeRuntime(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d).AttachedToVisualTree();
            }
        }

        private static void Control_DetachedFromVisualTreeRuntime(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is IAvaloniaObject d)
            {
                GetBehaviors(d).DetachedFromVisualTree();
            }
        }
    }

    public class BehaviorCollectionTest : AvaloniaList<IAvaloniaObject>
    {
        private readonly List<IBehavior> _oldCollection = new();

        public BehaviorCollectionTest()
        {
            CollectionChanged += BehaviorCollection_CollectionChanged;
        }

        public IAvaloniaObject? AssociatedObject
        {
            get;
            private set;
        }

        public void Attach(IAvaloniaObject? associatedObject)
        {
            if (Equals(associatedObject, AssociatedObject))
            {
                return;
            }

            if (AssociatedObject is { })
            {
                throw new InvalidOperationException(
                    "An instance of a behavior cannot be attached to more than one object at a time.");
            }

            Debug.Assert(associatedObject is { }, "The previous checks should keep us from ever setting null here.");
            AssociatedObject = associatedObject;

            foreach (var item in this)
            {
                var behavior = (IBehavior)item;
                behavior.Attach(AssociatedObject);
            }
        }

        public void Detach()
        {
            foreach (var item in this)
            {
                if (item is IBehavior { AssociatedObject: { } } behaviorItem)
                {
                    behaviorItem.Detach();
                }
            }

            AssociatedObject = null;
            _oldCollection.Clear();
        }

        internal void AttachedToVisualTree()
        {
            foreach (var item in this)
            {
                if (item is BehaviorTest behavior)
                {
                    behavior.AttachedToVisualTree();
                }
            }
        }

        internal void DetachedFromVisualTree()
        {
            foreach (var item in this)
            {
                if (item is BehaviorTest { AssociatedObject: { } } behavior)
                {
                    behavior.DetachedFromVisualTree();
                }
            }
        }

        private void BehaviorCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var behavior in _oldCollection)
                {
                    if (behavior.AssociatedObject is { })
                    {
                        behavior.Detach();
                    }
                }

                _oldCollection.Clear();

                foreach (var newItem in this)
                {
                    _oldCollection.Add(VerifiedAttach(newItem));
                }
#if DEBUG
                VerifyOldCollectionIntegrity();
#endif
                return;
            }

            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var eventIndex = eventArgs.NewStartingIndex;
                        var changedItem = eventArgs.NewItems?[0] as IAvaloniaObject;
                        _oldCollection.Insert(eventIndex, VerifiedAttach(changedItem));
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var eventIndex = eventArgs.OldStartingIndex;
                        eventIndex = eventIndex == -1 ? 0 : eventIndex;

                        var changedItem = eventArgs.NewItems?[0] as IAvaloniaObject;

                        var oldItem = _oldCollection[eventIndex];
                        if (oldItem.AssociatedObject is { })
                        {
                            oldItem.Detach();
                        }

                        _oldCollection[eventIndex] = VerifiedAttach(changedItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        var eventIndex = eventArgs.OldStartingIndex;

                        var oldItem = _oldCollection[eventIndex];
                        if (oldItem.AssociatedObject is { })
                        {
                            oldItem.Detach();
                        }

                        _oldCollection.RemoveAt(eventIndex);
                    }
                    break;

                default:
                    Debug.Assert(false, "Unsupported collection operation attempted.");
                    break;
            }
#if DEBUG
            VerifyOldCollectionIntegrity();
#endif
        }

        private IBehavior VerifiedAttach(IAvaloniaObject? item)
        {
            if (!(item is IBehavior behavior))
            {
                throw new InvalidOperationException(
                    $"Only {nameof(IBehavior)} types are supported in a {nameof(BehaviorCollectionTest)}.");
            }

            if (_oldCollection.Contains(behavior))
            {
                throw new InvalidOperationException(
                    $"Cannot add an instance of a behavior to a {nameof(BehaviorCollectionTest)} more than once.");
            }

            if (AssociatedObject is { })
            {
                behavior.Attach(AssociatedObject);
            }

            return behavior;
        }

        [Conditional("DEBUG")]
        private void VerifyOldCollectionIntegrity()
        {
            var isValid = Count == _oldCollection.Count;
            if (isValid)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (!Equals(this[i], _oldCollection[i]))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            Debug.Assert(isValid, "Referential integrity of the collection has been compromised.");
        }
    }

    public class BehaviorTest : AvaloniaObject, IBehavior
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
    }
}
