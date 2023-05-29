using System.Reactive;

namespace Mapsui.Interactivity;

public interface IInteractive
{
    IObservable<Unit> Invalidate { get; }

    IObservable<Unit> Canceling { get; }

    IEnumerable<MPoint> GetActiveVertices();

    void Starting(MapInfo? mapInfo);

    void Starting(MapInfo? mapInfo, double screenDistance);

    void Moving(MapInfo? mapInfo);

    void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

    void Hovering(MapInfo? mapInfo);

    void HoveringBegin(MapInfo? mapInfo);

    void HoveringEnd();

    void Cancel();
}
