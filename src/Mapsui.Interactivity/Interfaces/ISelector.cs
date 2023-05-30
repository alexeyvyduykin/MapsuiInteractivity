using Mapsui.Layers;

namespace Mapsui.Interactivity;

public interface ISelector : IInteractive
{
    void Selected(IFeature feature, ILayer layer);

    void Unselected();

    void HoveringBegin(IFeature feature, ILayer layer);

    IObservable<(IFeature Feature, ILayer Layer)> Select { get; }

    IObservable<(IFeature Feature, ILayer Layer)> Unselect { get; }

    IObservable<(IFeature Feature, ILayer Layer)> HoverBegin { get; }

    IObservable<(IFeature Feature, ILayer Layer)> HoverEnd { get; }
}
