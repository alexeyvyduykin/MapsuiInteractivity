using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mapsui.Interactivity;

public abstract class BaseInteractive : IInteractive
{
    private readonly Subject<IInteractive> _invalidateSubj = new();
    private readonly Subject<Unit> _cancelingSubj = new();

    public IObservable<IInteractive> Invalidate => _invalidateSubj.AsObservable();

    public IObservable<Unit> Canceling => _cancelingSubj.AsObservable();

    public abstract IEnumerable<MPoint> GetActiveVertices();

    public abstract IEnumerable<IFeature> GetFeatures();

    protected virtual void OnInvalidate()
    {
        _invalidateSubj.OnNext(this);
    }

    public virtual void Starting(MapInfo? mapInfo)
    {
        OnInvalidate();
    }

    public virtual void Starting(MapInfo? mapInfo, double screenDistance) { }

    public virtual void Moving(MapInfo? mapInfo) { }

    public virtual void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null) { }

    public virtual void Hovering(MapInfo? mapInfo) { }

    public virtual void HoveringBegin(MapInfo? mapInfo) { }

    public virtual void HoveringEnd() { }

    public virtual void Cancel()
    {
        _cancelingSubj.OnNext(Unit.Default);
    }
}
