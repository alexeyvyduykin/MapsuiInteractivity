using Mapsui.Interactivity.Interfaces;
using Mapsui.Layers;
using System.Reactive.Linq;

namespace Mapsui.Interactivity
{
    internal class DesignerBuilder : IDesignerBuilder
    {
        private readonly Func<IInteractive> _builder;

        public DesignerBuilder(Func<IInteractive> builder)
        {
            _builder = builder;
        }

        internal LayerCollection? Layers { get; set; }

        public IDesigner Build()
        {
            var designer = (IDesigner)_builder.Invoke();

            if (Layers != null)
            {
                designer = AttachDesigner(designer, Layers);
            }

            return designer;
        }

        public IDesignerBuilder AttachTo(LayerCollection layers)
        {
            Layers = layers;

            return this;
        }

        public IDesignerBuilder AttachTo(IMap map)
        {
            Layers = map.Layers;

            return this;
        }

        private static IDesigner AttachDesigner(IDesigner designer, LayerCollection layers)
        {
            InteractiveBuilder.RemoveInteractiveLayer(layers);

            var interactiveLayer = new InteractiveLayer(designer)
            {
                Name = nameof(InteractiveLayer),
                Style = InteractiveBuilder.CreateInteractiveLayerDesignerStyle(),
            };

            designer.BeginCreating.Subscribe(_ => layers.Add(interactiveLayer));

            designer.EndCreating.Subscribe(_ => layers.Remove(interactiveLayer));

            return designer;
        }
    }
}
