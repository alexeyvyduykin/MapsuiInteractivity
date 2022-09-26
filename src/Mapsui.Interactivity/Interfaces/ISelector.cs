using Mapsui.Layers;

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

        void PointeroverStart(IFeature feature, ILayer layer);
    }
}
