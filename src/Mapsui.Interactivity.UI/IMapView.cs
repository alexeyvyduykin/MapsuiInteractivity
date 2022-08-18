using Mapsui.Interactivity.UI.Input;
using Mapsui;
using Mapsui.UI;

namespace Mapsui.Interactivity.UI
{
    public interface IMapView : IView
    {
        IInteractiveBehavior Behavior { get; set; }

        IController Controller { get; set; }

        MPoint ScreenToWorld(MPoint screenPosition);

        MPoint WorldToScreen(MPoint worldPosition);

        MapInfo? GetMapInfo(MPoint screenPosition, int margin = 0);
    }
}