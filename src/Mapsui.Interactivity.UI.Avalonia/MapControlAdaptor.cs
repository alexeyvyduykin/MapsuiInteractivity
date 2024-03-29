﻿using Avalonia.Input;
using Mapsui.UI.Avalonia;

namespace Mapsui.Interactivity.UI.Avalonia;

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

    public void SetCursor(CursorType cursorType) => _mapControl.Cursor = new Cursor(cursorType.ToStandartCursor());
}
