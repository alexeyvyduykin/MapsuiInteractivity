using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.Interfaces;
using Mapsui.Layers;
using Mapsui.Nts;
using System.Reactive.Linq;

namespace Mapsui.Interactivity
{
    internal class SelectorWithDecoratorBuilder : ISelectorWithDecoratorBuilder
    {
        private readonly Func<GeometryFeature, IDecorator> _builder;
        private readonly LayerCollection? _layers;
        private IList<string>? _availableLayers;
        private readonly IList<ILayer> _dirtyLayers = new List<ILayer>();

        public SelectorWithDecoratorBuilder(LayerCollection? layers, Func<GeometryFeature, IDecorator> builder)
        {
            _builder = builder;
            _layers = layers;
        }

        public ISelectorWithDecoratorBuilder AvailableFor(ILayer[] layers)
        {
            _availableLayers = layers.Select(s => s.Name).ToList();

            return this;
        }

        public ISelectorWithDecoratorBuilder AvailableFor(ILayer layer)
        {
            _availableLayers = new List<string>() { layer.Name };

            return this;
        }

        public ISelector Build()
        {
            ISelector selector = new DecoratorSelector(_builder);

            if (_layers != null)
            {
                ((IDecoratorSelector)selector).DecoratorSelecting.Subscribe(s =>
                  _layers.AddInteractiveLayer(s, InteractiveBuilder.CreateInteractiveLayerDecoratorStyle()));

                selector.Unselect.Subscribe(_ => _layers.RemoveInteractiveLayer());
            }

            if (_availableLayers != null && _layers != null)
            {
                foreach (var item in _layers)
                {
                    if (_availableLayers.Contains(item.Name) == true)
                    {
                        if (item.IsMapInfoLayer == false)
                        {
                            item.IsMapInfoLayer = true;

                            _dirtyLayers.Add(item);
                        }
                    }
                    else
                    {
                        if (item.IsMapInfoLayer == true)
                        {
                            item.IsMapInfoLayer = false;

                            _dirtyLayers.Add(item);
                        }
                    }
                }
            }

            selector.Canceling.Subscribe(s =>
            {
                foreach (var item in _dirtyLayers)
                {
                    item.IsMapInfoLayer = !item.IsMapInfoLayer;
                }

                _dirtyLayers.Clear();
            });

            return selector;
        }
    }
}
