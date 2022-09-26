using Mapsui.Layers;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public interface ISelector : IInteractive
    {
        void Selected(IFeature feature, ILayer layer);

        void Unselected();

        void PointeroverStart(IFeature feature, ILayer layer);

        IFeature? SelectedFeature { get; }

        IFeature? HoveringFeature { get; }

        ReactiveCommand<Unit, ISelector> Select { get; }

        ReactiveCommand<Unit, ISelector> Unselect { get; }

        ReactiveCommand<Unit, ISelector> HoveringBegin { get; }

        ReactiveCommand<Unit, ISelector> HoveringEnd { get; }
    }
}
