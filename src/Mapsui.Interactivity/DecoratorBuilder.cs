﻿using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.Interfaces;
using Mapsui.Layers;
using Mapsui.Nts;
using System.Reactive.Linq;

namespace Mapsui.Interactivity;

internal class DecoratorBuilder : IDecoratorBuilder
{
    private readonly Func<GeometryFeature, IDecorator> _builder;

    public DecoratorBuilder(Func<GeometryFeature, IDecorator> builder)
    {
        _builder = builder;
    }

    internal LayerCollection? Layers { get; set; }

    internal GeometryFeature? GeometryFeature { get; set; }

    public IDecorator Build()
    {
        GeometryFeature ??= new GeometryFeature();

        var decorator = (IDecorator)_builder.Invoke(GeometryFeature);

        if (Layers != null)
        {
            Layers.AddInteractiveLayer(decorator, InteractiveBuilder.CreateInteractiveLayerDecoratorStyle());

            decorator.Canceling.Subscribe(_ => Layers.RemoveInteractiveLayer());
        }

        return decorator;
    }

    public ISelectorWithDecoratorBuilder WithSelector<T>() where T : ISelector
    {
        return new SelectorWithDecoratorBuilder(Layers, _builder);
    }

    public IDecoratorBuilder AttachTo(LayerCollection layers)
    {
        Layers = layers;

        return this;
    }

    public IDecoratorBuilder AttachTo(Map map)
    {
        Layers = map.Layers;

        return this;
    }

    public IDecoratorBuilder WithFeature(GeometryFeature feature)
    {
        GeometryFeature = feature;

        return this;
    }
}
