using Mapsui.UI;

namespace Mapsui.Interactivity.UI
{
    public interface IView
    {
        Map? Map { get; }

        void SetCursor(CursorType cursorType);

        IInteractiveBehavior Behavior { get; set; }

        MPoint ScreenToWorld(MPoint screenPosition);

        MPoint WorldToScreen(MPoint worldPosition);

        MapInfo? GetMapInfo(MPoint screenPosition, int margin = 0);
    }
}