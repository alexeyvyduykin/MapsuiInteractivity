using Mapsui.Interactivity.Extensions;
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
                designer.BeginCreating.Subscribe(s => Layers.AddInteractiveLayer(s, InteractiveBuilder.CreateInteractiveLayerDesignerStyle()));

                designer.EndCreating.Subscribe(_ => Layers.RemoveInteractiveLayer());
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
    }
}
