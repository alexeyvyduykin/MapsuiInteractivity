using Mapsui.Nts;
using System.Reactive;

namespace Mapsui.Interactivity;

public interface IDecoratorSelector : ISelector
{
    IObservable<IDecorator> DecoratorSelecting { get; }

    IObservable<GeometryFeature?> DecoratorUnselecting { get; }
}
