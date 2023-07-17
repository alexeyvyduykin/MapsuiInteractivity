using Mapsui.Interactivity.UI.Wpf.Extensions;
using Mapsui.UI.Wpf;

namespace Mapsui.Interactivity.UI.Wpf;

internal class MapControlAdaptor : IView
{
    private readonly MapControl _mapControl;
    private readonly IInteractive _interactive;

    public MapControlAdaptor(MapControl mapControl, IInteractive interactive)
    {
        _mapControl = mapControl;
        _interactive = interactive;
    }

    public Navigator Navigator => _mapControl.Map.Navigator;

    public IInteractive Interactive => _interactive;

    public void SetCursor(Mapsui.Interactivity.UI.CursorType cursorType) => _mapControl.Cursor = cursorType.ToStandartCursor();
}
