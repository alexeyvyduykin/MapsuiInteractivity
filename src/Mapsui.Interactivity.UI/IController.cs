using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    public interface IMapController : IController
    {
    }

    public interface IController
    {
        bool HandleMouseDown(IView view, MouseDownEventArgs args);

        bool HandleMouseMove(IView view, MouseEventArgs args);

        bool HandleMouseUp(IView view, MouseEventArgs args);

        bool HandleMouseEnter(IView view, MouseEventArgs args);

        bool HandleMouseLeave(IView view, MouseEventArgs args);

        bool HandleMouseWheel(IView view, MouseWheelEventArgs args);

        bool HandleGesture(IView view, InputGesture gesture, InputEventArgs args);

        void AddMouseManipulator(IView view, ManipulatorBase<MouseEventArgs> manipulator, MouseDownEventArgs args);

        void AddHoverManipulator(IView view, ManipulatorBase<MouseEventArgs> manipulator, MouseEventArgs args);

        void Bind(MouseDownGesture gesture, IViewCommand<MouseDownEventArgs> command);

        void Bind(MouseEnterGesture gesture, IViewCommand<MouseEventArgs> command);

        void Bind(MouseWheelGesture gesture, IViewCommand<MouseWheelEventArgs> command);

        void Unbind(InputGesture gesture);

        void Unbind(IViewCommand command);

        void UnbindAll();
    }
}