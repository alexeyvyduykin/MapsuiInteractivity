using Mapsui;

namespace Mapsui.Interactivity.UI
{
    public interface IView
    {
        Map? Map { get; }

        void SetCursor(CursorType cursorType);
    }
}