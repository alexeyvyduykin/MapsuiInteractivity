using Mapsui.Layers;
using Mapsui.Nts;

namespace Mapsui.Interactivity
{
    public partial class InteractiveBuilder
    {
        internal GeometryFeature? GeometryFeature { get; set; }

        internal LayerCollection? Layers { get; set; }

        internal Type? WrappedDecoratorType { get; set; }

        public InteractiveBuilder AttachTo(LayerCollection layers)
        {
            Layers = layers;

            return this;
        }

        public InteractiveBuilder AttachTo(IMap map)
        {
            Layers = map.Layers;

            return this;
        }

        public InteractiveBuilder Wrapped<T>() where T : IDecorator
        {
            WrappedDecoratorType = typeof(T);

            return this;
        }
    }
}
