using Mapsui.Nts;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity;

public interface IDesigner : IInteractive
{
    GeometryFeature Feature { get; }

    IList<GeometryFeature> ExtraFeatures { get; }

    ReactiveCommand<Unit, IDesigner> BeginCreating { get; }

    ReactiveCommand<Unit, IDesigner> Creating { get; }

    ReactiveCommand<Unit, IDesigner> HoverCreating { get; }

    ReactiveCommand<Unit, IDesigner> EndCreating { get; }
}
