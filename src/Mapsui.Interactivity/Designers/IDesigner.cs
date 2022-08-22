using Mapsui.Nts;

namespace Mapsui.Interactivity
{
    public interface IDesigner : IInteractive
    {
        GeometryFeature Feature { get; }

        IList<GeometryFeature> ExtraFeatures { get; }

        event EventHandler? BeginCreating;

        event EventHandler? Creating;

        event EventHandler? HoverCreating;

        event EventHandler? EndCreating;
    }
}
