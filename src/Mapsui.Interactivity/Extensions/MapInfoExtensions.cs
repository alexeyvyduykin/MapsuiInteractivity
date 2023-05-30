namespace Mapsui.Interactivity.Extensions;

public static class MapInfoExtensions
{
    public static bool IsInteractiveLayer(this MapInfo mapInfo)
    {
        return mapInfo.Layer != null && mapInfo.Layer is InteractiveLayer;
    }
}
