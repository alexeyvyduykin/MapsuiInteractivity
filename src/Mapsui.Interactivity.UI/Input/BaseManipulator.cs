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

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            var currentPosition = e.Position;

            if (IsClick(currentPosition, StartPosition) == true)
            {
                View.Behavior?.OnClick(e.MapInfo);
            }
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);
            StartPosition = e.Position;
        }

        private static bool IsClick(MPoint? currentPosition, MPoint? previousPosition)
        {
            if (currentPosition == null || previousPosition == null || currentPosition.Equals(new MPoint(0.0, 0.0)))
                return false;

            return
                Math.Abs(currentPosition.X - previousPosition.X) < 1 &&
                Math.Abs(currentPosition.Y - previousPosition.Y) < 1;
        }
    }
}