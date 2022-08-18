using Mapsui.Interactivity.UI.Input.Core;
using Mapsui;

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

    public abstract class MapManipulator<T> : ManipulatorBase<T> where T : InputEventArgs
    {
        protected MapManipulator(IMapView view) : base(view)
        {
            MapView = view;
        }

        public IMapView MapView { get; private set; }
    }

    public abstract class MouseManipulator : MapManipulator<MouseEventArgs>
    {
        protected MouseManipulator(IMapView plotView)
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