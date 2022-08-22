namespace Mapsui.Interactivity.UI
{
    public interface IView
    {
        Map? Map { get; }

        void SetCursor(CursorType cursorType);

        IInteractiveBehavior Behavior { get; set; }

        MPoint WorldToScreen(MPoint worldPosition);
    }
}