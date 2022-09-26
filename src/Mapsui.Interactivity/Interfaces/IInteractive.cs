using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public interface IInteractive
    {
        event EventHandler? InvalidateLayer;

        event EventHandler? Cancel;

        IEnumerable<MPoint> GetActiveVertices();

        void Starting(MapInfo? mapInfo);

        void Moving(MapInfo? mapInfo);

        void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        void Hovering(MapInfo? mapInfo);

        void Canceling();

        void Starting(MapInfo? mapInfo, double screenDistance);

        void PointeroverStart(MapInfo? mapInfo);

        void PointeroverStop();
    }
}
