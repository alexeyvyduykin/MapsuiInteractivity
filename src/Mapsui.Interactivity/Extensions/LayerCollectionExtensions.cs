using Mapsui.Layers;
using Mapsui.Styles;

namespace Mapsui.Interactivity.Extensions
{
    public static class LayerCollectionExtensions
    {
        public static InteractiveLayer AddInteractiveLayer(this LayerCollection layers, IInteractive interactive, IStyle style)
        {
            layers.RemoveInteractiveLayer();

            var interactiveLayer = new InteractiveLayer(interactive)
            {
                Name = nameof(InteractiveLayer),
                Style = style,
            };

            layers.Add(interactiveLayer);

            return interactiveLayer;
        }

        public static void RemoveInteractiveLayer(this LayerCollection layers)
        {
            var interactiveLayer = layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                // HACK: before remove from layers clear interactive geometries (mapsui not auto clear data)
                ((InteractiveLayer)interactiveLayer).Cancel();

                layers.Remove(interactiveLayer);
            }
        }
    }
}
