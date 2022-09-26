using Mapsui.Nts;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public interface IDesigner : IInteractive
    {
        GeometryFeature Feature { get; }

        IList<GeometryFeature> ExtraFeatures { get; }

        event EventHandler? BeginCreating;

        event EventHandler? Creating;

        ReactiveCommand<Unit, IDesigner> HoverCreating { get; }

        event EventHandler? EndCreating;
    }
}
