using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI.Input
{
    public abstract class ManipulatorBase<T> where T : InputEventArgs
    {
        protected ManipulatorBase(IView view)
        {
            View = view;
        }

        public IView View { get; private set; }

        public virtual void Completed(T e) { }

        public virtual void Delta(T e) { }

        public virtual void Started(T e) { }
    }

    public abstract class MouseManipulator : ManipulatorBase<MouseEventArgs>
    {
        protected MouseManipulator(IView plotView)
            : base(plotView)
        {
        }

        public MPoint? StartPosition { get; protected set; }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);
            StartPosition = e.Position;
        }
    }
}