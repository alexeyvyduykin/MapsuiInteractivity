using Avalonia.Input;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia
{
    internal class MapControlAdaptor : IView
    {
        private readonly MapControl _mapControl;
        private readonly InteractiveBehavior _behavior;

        public MapControlAdaptor(MapControl mapControl, InteractiveBehavior behavior)
        {
            _mapControl = mapControl;
            _behavior = behavior;
        }

        public Map? Map => _mapControl.Map;

        public IInteractiveBehavior Behavior => _behavior;

        public void SetCursor(CursorType cursorType) => _mapControl.Cursor = new Cursor(cursorType.ToStandartCursor());

        public MPoint WorldToScreen(MPoint worldPosition) => _mapControl.Viewport.WorldToScreen(worldPosition);
    }
}
