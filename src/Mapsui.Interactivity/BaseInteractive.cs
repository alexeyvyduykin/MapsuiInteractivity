using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public abstract class BaseInteractive : IInteractive
    {
        public event EventHandler? InvalidateLayer;
        public event EventHandler? Cancel;

        protected void Invalidate()
        {
            InvalidateLayer?.Invoke(this, EventArgs.Empty);
        }

        public void Canceling()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }

        public abstract IEnumerable<MPoint> GetActiveVertices();

        public virtual void Starting(MapInfo? mapInfo)
        {

        }

        public virtual void Moving(MapInfo? mapInfo)
        {

        }

        public abstract void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        public virtual void Hovering(MapInfo? mapInfo)
        {

        }

        public virtual void Starting(MapInfo? mapInfo, double screenDistance)
        {

        }

        public virtual void PointeroverStart(MapInfo? mapInfo)
        {

        }

        public virtual void PointeroverStop()
        {

        }
    }
}
