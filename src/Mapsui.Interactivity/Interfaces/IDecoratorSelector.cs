using ReactiveUI;

namespace Mapsui.Interactivity
{
    public interface IDecoratorSelector : ISelector
    {
        ReactiveCommand<IDecorator, IDecorator> DecoratorSelecting { get; }
    }
}
