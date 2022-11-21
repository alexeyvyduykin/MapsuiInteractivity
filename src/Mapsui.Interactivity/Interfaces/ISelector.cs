using Mapsui.Layers;
using ReactiveUI;

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

    ReactiveCommand<ISelector, ISelector> Select { get; }

    ReactiveCommand<ISelector, ISelector> Unselect { get; }

    ReactiveCommand<ISelector, ISelector> HoverBegin { get; }

    ReactiveCommand<ISelector, ISelector> HoverEnd { get; }
}
