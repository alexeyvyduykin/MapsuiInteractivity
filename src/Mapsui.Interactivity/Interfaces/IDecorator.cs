using Mapsui.Nts;

namespace Mapsui.Interactivity;

public interface IDecorator : IInteractive
{
    GeometryFeature FeatureSource { get; }

    bool IsFeatureChange { get; }
}
