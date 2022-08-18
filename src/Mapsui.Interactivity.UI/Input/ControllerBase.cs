using Mapsui.Interactivity.UI.Input.Core;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Interactivity.UI.Input
{
    public abstract class ControllerBase : IController
    {
        private readonly object syncRoot = new();

        protected ControllerBase()
        {
            InputCommandBindings = new List<InputCommandBinding>();
            MouseDownManipulators = new List<ManipulatorBase<MouseEventArgs>>();
            MouseHoverManipulators = new List<ManipulatorBase<MouseEventArgs>>();
        }

        public List<InputCommandBinding> InputCommandBindings { get; private set; }

        protected IList<ManipulatorBase<MouseEventArgs>> MouseDownManipulators { get; private set; }

        protected IList<ManipulatorBase<MouseEventArgs>> MouseHoverManipulators { get; private set; }

        public virtual bool HandleGesture(IView view, InputGesture gesture, InputEventArgs args)
        {
            var command = GetCommand(gesture);
            return HandleCommand(command, view, args);
        }

        public virtual bool HandleMouseEnter(IView view, MouseEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                if (view.Map != null)
                {                  
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                var command = GetCommand(new MouseEnterGesture());
                return HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleMouseLeave(IView view, MouseEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                if (view.Map != null)
                {              
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in MouseHoverManipulators.ToArray())
                {
                    m.Completed(args);
                    MouseHoverManipulators.Remove(m);
                }

                return true;
            }
        }

        public virtual bool HandleMouseDown(IView view, MouseDownEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                if (view.Map != null)
                {                
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                var command = GetCommand(new MouseDownGesture(args.ChangedButton, args.ClickCount));
                return HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleMouseMove(IView view, MouseEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                if (args.Handled)
                {
                    return true;
                }

                if (view.Map != null)
                {               
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in MouseDownManipulators)
                {
                    m.Delta(args);
                }

                foreach (var m in MouseHoverManipulators)
                {
                    m.Delta(args);
                }

                return true;
            }
        }

        public virtual bool HandleMouseUp(IView view, MouseEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                if (args.Handled)
                {
                    return true;
                }

                if (view.Map != null)
                {          
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in MouseDownManipulators.ToArray())
                {
                    m.Completed(args);
                    MouseDownManipulators.Remove(m);
                }

                return true;
            }
        }

        public virtual bool HandleMouseWheel(IView view, MouseWheelEventArgs args)
        {
            lock (GetSyncRoot(view))
            {
                var command = GetCommand(new MouseWheelGesture());
                return HandleCommand(command, view, args);
            }
        }

        public virtual void AddMouseManipulator(
            IView view,
            ManipulatorBase<MouseEventArgs> manipulator,
            MouseDownEventArgs args)
        {
            MouseDownManipulators.Add(manipulator);
            manipulator.Started(args);
        }

        public virtual void AddHoverManipulator(
            IView view,
            ManipulatorBase<MouseEventArgs> manipulator,
            MouseEventArgs args)
        {
            MouseHoverManipulators.Add(manipulator);
            manipulator.Started(args);
        }

        public virtual void Bind(MouseDownGesture gesture, IViewCommand<MouseDownEventArgs> command)
        {
            BindCore(gesture, command);
        }

        public virtual void Bind(MouseEnterGesture gesture, IViewCommand<MouseEventArgs> command)
        {
            BindCore(gesture, command);
        }

        public virtual void Bind(MouseWheelGesture gesture, IViewCommand<MouseWheelEventArgs> command)
        {
            BindCore(gesture, command);
        }

        public virtual void Unbind(InputGesture gesture)
        {
            // ReSharper disable once RedundantNameQualifier
            foreach (var icb in InputCommandBindings.Where(icb => icb.Gesture.Equals(gesture)).ToArray())
            {
                InputCommandBindings.Remove(icb);
            }
        }

        public virtual void Unbind(IViewCommand command)
        {
            // ReSharper disable once RedundantNameQualifier
            foreach (var icb in InputCommandBindings.Where(icb => object.ReferenceEquals(icb.Command, command)).ToArray())
            {
                InputCommandBindings.Remove(icb);
            }
        }

        public virtual void UnbindAll()
        {
            InputCommandBindings.Clear();
        }

        protected void BindCore(InputGesture gesture, IViewCommand command)
        {
            var current = InputCommandBindings.FirstOrDefault(icb => icb.Gesture.Equals(gesture));
            if (current != null)
            {
                InputCommandBindings.Remove(current);
            }

            if (command != null)
            {
                InputCommandBindings.Add(new InputCommandBinding(gesture, command));
            }
        }

        protected virtual IViewCommand? GetCommand(InputGesture gesture)
        {
            var binding = InputCommandBindings.FirstOrDefault(b => b.Gesture.Equals(gesture));
            if (binding == null)
            {
                return null;
            }

            return binding.Command;
        }

        protected virtual bool HandleCommand(IViewCommand? command, IView view, InputEventArgs args)
        {
            if (command == null)
            {
                return false;
            }

            command.Execute(view, this, args);

            args.Handled = true;
            return true;
        }

        protected object GetSyncRoot(IView _)
        {
            return syncRoot;
        }
    }
}