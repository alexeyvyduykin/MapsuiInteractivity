using Mapsui.Nts;

namespace Mapsui.Interactivity;

public interface IDesigner : IInteractive
{
    GeometryFeature Feature { get; }

    IList<GeometryFeature> ExtraFeatures { get; }

    IObservable<IDesigner> BeginCreating { get; }

    IObservable<IDesigner> Creating { get; }

    IObservable<IDesigner> HoverCreating { get; }

    IObservable<GeometryFeature> EndCreating { get; }
}
