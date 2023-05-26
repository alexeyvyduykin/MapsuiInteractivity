using Mapsui.Layers;
using Mapsui.Nts;

namespace Mapsui.Interactivity.Interfaces;

public interface IBuilder { }

public interface IBuilder<T> : IBuilder where T : IInteractive
{
    T Build();
}

public interface IAttachable<T> where T : IBuilder
{
    T AttachTo(LayerCollection layers);

    T AttachTo(Map map);
}

public interface IFilterable<T> where T : IBuilder
{
    T AvailableFor(ILayer[] layers);

    T AvailableFor(ILayer layer);
}

public interface ISelectorBuilder : IBuilder<ISelector>, IAttachable<ISelectorBuilder>, IFilterable<ISelectorBuilder> { }

public interface IDesignerBuilder : IBuilder<IDesigner>, IAttachable<IDesignerBuilder> { }

public interface IDecoratorBuilder : IBuilder<IDecorator>, IAttachable<IDecoratorBuilder>
{
    ISelectorWithDecoratorBuilder WithSelector<T>() where T : ISelector;

    IDecoratorBuilder WithFeature(GeometryFeature feature);
}

public interface ISelectorWithDecoratorBuilder : IBuilder<ISelector>, IFilterable<ISelectorWithDecoratorBuilder> { }
