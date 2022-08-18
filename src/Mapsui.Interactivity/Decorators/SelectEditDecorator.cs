using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using System;

namespace Mapsui.Interactivity
{
    public interface ISelectEditDecorator : ISelectDecorator
    {
        IDecorator? Edit { get; }

        event EventHandler? DecoratorChanged;
    }

    internal class SelectEditDecorator : SelectDecorator, ISelectEditDecorator
    {
        private IDecorator? _decorator;

        public SelectEditDecorator(Map map, ILayer layer) : base(map, layer)
        {

        }

        public event EventHandler? DecoratorChanged;

        protected override void SelectImpl(IFeature feature)
        {
            if (feature is GeometryFeature gf && gf.Geometry is not Point)
            {
                var decorator = new InteractiveFactory().CreateEditDecorator(gf);

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

        public IDecorator? Edit => _decorator;
    }
}
