using Mapsui.Layers;
using Mapsui.Nts;

namespace Mapsui.Interactivity.Interfaces
{
    public interface IBuilder { }

    public interface IBuilder<T> : IBuilder where T : IInteractive
    {
        T Build();
    }

    public interface IAttachable<T> where T : IBuilder
    {
        T AttachTo(LayerCollection layers);

        T AttachTo(IMap map);
    }

    public interface ISelectorBuilder : IBuilder<ISelector> { }

    public interface IDesignerBuilder : IBuilder<IDesigner>, IAttachable<IDesignerBuilder> { }

    public interface IDecoratorBuilder : IBuilder<IDecorator>, IAttachable<IDecoratorBuilder>
    {
        ISelectorWithDecoratorBuilder WithSelector<T>() where T : ISelector;

        IDecoratorBuilder WithFeature(GeometryFeature feature);
    }

    public interface ISelectorWithDecoratorBuilder : ISelectorBuilder { }
}
