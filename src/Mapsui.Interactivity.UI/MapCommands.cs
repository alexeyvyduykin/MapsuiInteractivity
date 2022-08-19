using Mapsui.Interactivity.UI.Input.Core;

namespace Mapsui.Interactivity.UI
{
    internal static class MapCommands
    {
        static MapCommands()
        {
            Default = new DelegateViewCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DefaultManipulator(view), args));
            HoverDefault = new DelegateViewCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDefaultManipulator(view), args));

            Editing = new DelegateViewCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator(view), args));
            HoverEditing = new DelegateViewCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator(view), args));

            Drawing = new DelegateViewCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingManipulator(view), args));
            HoverDrawing = new DelegateViewCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingManipulator(view), args));
        }

        public static IViewCommand<MouseDownEventArgs> Default { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDefault { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Drawing { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawing { get; private set; }
    }
}