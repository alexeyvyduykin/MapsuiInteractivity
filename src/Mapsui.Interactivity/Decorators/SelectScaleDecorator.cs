using Mapsui.Layers;
using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public interface ISelectScaleDecorator : ISelectDecorator
    {
        IDecorator? Scale { get; }

        event EventHandler? DecoratorChanged;
    }

    internal class SelectScaleDecorator : SelectDecorator, ISelectScaleDecorator
    {
        private IDecorator? _decorator;

        public SelectScaleDecorator(Map map, ILayer layer) : base(map, layer)
        {

        }

        public event EventHandler? DecoratorChanged;

        protected override void SelectImpl(IFeature feature)
        {
            if (feature is GeometryFeature gf && gf.Geometry is not Point)
            {
                var decorator = new InteractiveFactory().CreateScaleDecorator(gf);

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

        public IDecorator? Scale => _decorator;
    }
}
