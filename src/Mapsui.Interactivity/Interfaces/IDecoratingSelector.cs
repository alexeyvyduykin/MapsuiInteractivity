namespace Mapsui.Interactivity
{
    public interface IDecoratingSelector : ISelector
    {
        IDecorator? Decorator { get; }

        public event EventHandler? SelectedDecorator;
    }
}
