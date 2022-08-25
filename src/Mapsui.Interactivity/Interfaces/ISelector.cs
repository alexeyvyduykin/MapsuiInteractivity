using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public interface ISelector : IInteractive
    {
        public event EventHandler? Select;

        public event EventHandler? Unselect;

        public event EventHandler? HoveringBegin;

        public event EventHandler? HoveringEnd;

        void PointeroverStart(MapInfo? mapInfo);

        void PointeroverStop(MapInfo? mapInfo);

        void Unselected();
    }
}
