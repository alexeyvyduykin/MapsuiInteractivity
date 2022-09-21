using Mapsui.Layers;
using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public interface ISelector : IInteractive
    {
        event EventHandler? Select;

        event EventHandler? Unselect;

        event EventHandler? HoveringBegin;

        event EventHandler? HoveringEnd;

        void Selected(IFeature feature, ILayer layer);

        void Unselected();

        void PointeroverStart(MapInfo? mapInfo);

        void PointeroverStart(IFeature feature, ILayer layer);

        void PointeroverStop();
    }
}
