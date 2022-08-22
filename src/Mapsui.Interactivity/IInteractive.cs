using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public interface IInteractive
    {
        event EventHandler? InvalidateLayer;

        IEnumerable<MPoint> GetActiveVertices();

        void Starting(MPoint worldPosition);

        void Moving(MPoint worldPosition);

        void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        void Hovering(MapInfo? mapInfo);

        void Dispose(MapInfo? mapInfo);
    }
}
