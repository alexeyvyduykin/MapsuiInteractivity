﻿using Mapsui.Interactivity.Extensions;
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

        public SelectorWithDecoratorBuilder(LayerCollection? layers, Func<GeometryFeature, IDecorator> builder)
        {
            _builder = builder;
            _layers = layers;
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

            return selector;
        }
    }
}