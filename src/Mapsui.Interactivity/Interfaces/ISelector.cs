using Mapsui.Layers;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public interface ISelector : IInteractive
    {
        void Selected(IFeature feature, ILayer layer);

        void Unselected();

        void HoveringBegin(IFeature feature, ILayer layer);

        IFeature? SelectedFeature { get; }

        IFeature? HoveringFeature { get; }

        ReactiveCommand<Unit, ISelector> Select { get; }

        ReactiveCommand<Unit, ISelector> Unselect { get; }

        ReactiveCommand<Unit, ISelector> HoverBegin { get; }

        ReactiveCommand<Unit, ISelector> HoverEnd { get; }
    }
}
