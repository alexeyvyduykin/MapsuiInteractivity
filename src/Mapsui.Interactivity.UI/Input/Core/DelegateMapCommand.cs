namespace Mapsui.Interactivity.UI.Input.Core
{
    public class DelegateMapCommand<T> : DelegateViewCommand<T> where T : InputEventArgs
    {
        public DelegateMapCommand(Action<IView, IController, T> handler)
            : base((v, c, e) => handler(v, c, e)) { }
    }
}