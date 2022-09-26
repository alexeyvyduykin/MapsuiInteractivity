using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public interface IDecoratingSelector : ISelector
    {
        ReactiveCommand<Unit, IDecorator> DecoratorSelecting { get; }
    }
}
