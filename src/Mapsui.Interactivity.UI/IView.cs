namespace Mapsui.Interactivity.UI;

public interface IView
{
    Navigator Navigator { get; }

    IInteractive Interactive { get; }

    void SetCursor(CursorType cursorType);
}