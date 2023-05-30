using Mapsui.Layers;
using Mapsui.Styles;

namespace Mapsui.Interactivity.Extensions;

public static class LayerCollectionExtensions
{
    public static void AddInteractiveLayer(this LayerCollection layers, IInteractive interactive, IStyle style)
    {
        layers.RemoveInteractiveLayer();

        var interactiveLayer = new InteractiveLayer(interactive)
        {
            Name = nameof(InteractiveLayer),
            Style = style,
        };

        layers.Add(interactiveLayer);
    }

    public static void RemoveInteractiveLayer(this LayerCollection layers)
    {
        var layer = layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

        if (layer is InteractiveLayer interactiveLayer)
        {
            // HACK: before remove from layers clear interactive geometries (mapsui not auto clear data)
            interactiveLayer.Cancel();

            layers.Remove(interactiveLayer);

            interactiveLayer.Dispose();
        }
    }
}
