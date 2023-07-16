using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.UI.Input;
using wpfInput = System.Windows.Input;

namespace InteractivityWPFSample.Extensions;

public static class WPFExtensions
{
    public static wpfInput.Cursor ToStandartCursor(this CursorType cursorType)
    {
        return cursorType switch
        {
            CursorType.Default => wpfInput.Cursors.Arrow,
            CursorType.Hand => wpfInput.Cursors.Hand,
            CursorType.HandGrab => wpfInput.Cursors.SizeAll,
            CursorType.Cross => wpfInput.Cursors.Cross,
            _ => throw new System.Exception(),
        };
    }

    public static MouseButton Convert(this wpfInput.MouseButton state)
    {
        return state switch
        {
            wpfInput.MouseButton.Left => MouseButton.Left,
            wpfInput.MouseButton.Middle => MouseButton.Middle,
            wpfInput.MouseButton.Right => MouseButton.Right,
            _ => MouseButton.None,
        };
    }
}
