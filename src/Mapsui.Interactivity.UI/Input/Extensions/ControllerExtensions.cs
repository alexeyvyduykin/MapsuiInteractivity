using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI.Input
{
    public static class ControllerExtensions
    {
        public static void BindMouseDown(this IController controller, MouseButton mouseButton, IViewCommand<MouseDownEventArgs> command)
        {
            controller.Bind(new MouseDownGesture(mouseButton), command);
        }

        public static void BindMouseDown(this IController controller, MouseButton mouseButton, int clickCount, IViewCommand<MouseDownEventArgs> command)
        {
            controller.Bind(new MouseDownGesture(mouseButton, clickCount), command);
        }

        public static void BindMouseEnter(this IController controller, IViewCommand<MouseEventArgs> command)
        {
            controller.Bind(new MouseEnterGesture(), command);
        }

        public static void BindMouseWheel(this IController controller, IViewCommand<MouseWheelEventArgs> command)
        {
            controller.Bind(new MouseWheelGesture(), command);
        }

        public static void UnbindMouseDown(this IController controller, MouseButton mouseButton, int clickCount = 1)
        {
            controller.Unbind(new MouseDownGesture(mouseButton, clickCount));
        }

        public static void UnbindMouseEnter(this IController controller)
        {
            controller.Unbind(new MouseEnterGesture());
        }

        public static void UnbindMouseWheel(this IController controller)
        {
            controller.Unbind(new MouseWheelGesture());
        }
    }
}