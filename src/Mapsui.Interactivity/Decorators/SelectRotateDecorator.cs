using Mapsui.Layers;
using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public interface ISelectRotateDecorator : ISelectDecorator
    {
        IDecorator? Rotate { get; }

        event EventHandler? DecoratorChanged;
    }

    internal class SelectRotateDecorator : SelectDecorator, ISelectRotateDecorator
    {
        private IDecorator? _decorator;

        public SelectRotateDecorator(Map map, ILayer layer) : base(map, layer)
        {

        }

        public event EventHandler? DecoratorChanged;

        protected override void SelectImpl(IFeature feature)
        {
            if (feature is GeometryFeature gf && gf.Geometry is not Point)
            {
                var decorator = new InteractiveFactory().CreateRotateDecorator(gf);

                UpdateDecorator(decorator);

                base.SelectImpl(feature);
            }
        }

        protected override void UnselectImpl()
        {
            UpdateDecorator(null);

            base.UnselectImpl();
        }

        private void UpdateDecorator(IDecorator? decorator)
        {
            _decorator = decorator;

            DecoratorChanged?.Invoke(this, EventArgs.Empty);
        }

        public IDecorator? Rotate => _decorator;
    }
}
