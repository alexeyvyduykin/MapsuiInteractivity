using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI.Input
{
    public abstract class BaseManipulator<T> where T : InputEventArgs
    {
        protected BaseManipulator(IView view)
        {
            View = view;
        }

        public IView View { get; private set; }

        public virtual void Completed(T e) { }

        public virtual void Delta(T e) { }

        public virtual void Started(T e) { }
    }

    public abstract class MouseManipulator : BaseManipulator<MouseEventArgs>
    {
        protected MouseManipulator(IView view) : base(view) { }

        public MPoint? StartPosition { get; protected set; }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);
            StartPosition = e.Position;
        }
    }
}