using Mapsui.UI;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public abstract class BaseInteractive : IInteractive
    {
        public BaseInteractive()
        {
            Invalidate = ReactiveCommand.Create(() => { }, outputScheduler: RxApp.MainThreadScheduler);
            Canceling = ReactiveCommand.Create(() => { }, outputScheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<Unit, Unit> Invalidate { get; }

        public ReactiveCommand<Unit, Unit> Canceling { get; }

        public abstract IEnumerable<MPoint> GetActiveVertices();

        public virtual void Starting(MapInfo? mapInfo) { }

        public virtual void Starting(MapInfo? mapInfo, double screenDistance) { }

        public virtual void Moving(MapInfo? mapInfo) { }

        public virtual void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null) { }

        public virtual void Hovering(MapInfo? mapInfo) { }

        public virtual void HoveringBegin(MapInfo? mapInfo) { }

        public virtual void HoveringEnd() { }

        public virtual void Cancel()
        {
            Canceling.Execute().Subscribe();
        }
    }
}
