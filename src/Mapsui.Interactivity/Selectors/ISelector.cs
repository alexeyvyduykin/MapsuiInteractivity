using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public interface ISelector : IInteractive
    {
        public event EventHandler? Selecting;

        public event EventHandler? Unselecting;

        void Click(MapInfo? mapInfo);

        void PointeroverStart(MapInfo? mapInfo);

        void PointeroverStop(MapInfo? mapInfo);
    }
}
