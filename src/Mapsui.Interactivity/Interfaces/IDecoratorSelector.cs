namespace Mapsui.Interactivity;

public interface IDecoratorSelector : ISelector
{
    IObservable<IDecorator> DecoratorSelecting { get; }
}
