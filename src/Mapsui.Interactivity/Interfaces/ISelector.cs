using Mapsui.Layers;

namespace Mapsui.Interactivity;

public interface ISelector : IInteractive
{
    void Selected(IFeature feature, ILayer layer);

    void Unselected();

    void HoveringBegin(IFeature feature, ILayer layer);

    IFeature? SelectedFeature { get; }

    ILayer? SelectedLayer { get; }

    IFeature? HoveringFeature { get; }

    ILayer? PointeroverLayer { get; }

    IObservable<ISelector> Select { get; }

    IObservable<ISelector> Unselect { get; }

    IObservable<ISelector> HoverBegin { get; }

    IObservable<ISelector> HoverEnd { get; }
}
