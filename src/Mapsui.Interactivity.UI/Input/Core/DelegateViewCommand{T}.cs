using System;

namespace Mapsui.Interactivity.UI.Input.Core
{
    public class DelegateViewCommand<T> : IViewCommand<T> where T : InputEventArgs
    {
        private readonly Action<IView, IController, T> handler;

        public DelegateViewCommand(Action<IView, IController, T> handler)
        {
            this.handler = handler;
        }

        public void Execute(IView view, IController controller, T args)
        {
            handler(view, controller, args);
        }

        public void Execute(IView view, IController controller, InputEventArgs args)
        {
            handler(view, controller, (T)args);
        }
    }
}