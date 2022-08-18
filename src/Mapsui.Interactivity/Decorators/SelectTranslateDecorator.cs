using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using System;

namespace Mapsui.Interactivity
{
    public interface ISelectTranslateDecorator : ISelectDecorator
    {
        IDecorator? Translate { get; }

        event EventHandler? DecoratorChanged;
    }

    internal class SelectTranslateDecorator : SelectDecorator, ISelectTranslateDecorator
    {
        private IDecorator? _decorator;

        public SelectTranslateDecorator(Map map, ILayer layer) : base(map, layer)
        {

        }

        public event EventHandler? DecoratorChanged;

        protected override void SelectImpl(IFeature feature)
        {
            if (feature is GeometryFeature gf)
            {
                var decorator = new InteractiveFactory().CreateTranslateDecorator(gf);

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

        public IDecorator? Translate => _decorator;
    }
}
