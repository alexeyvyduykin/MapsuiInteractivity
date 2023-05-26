namespace Mapsui.Interactivity.UI;

public interface IView
{
    Map? Map { get; }

    void SetCursor(CursorType cursorType);

    IInteractive Interactive { get; }

    MPoint WorldToScreen(MPoint worldPosition);
}