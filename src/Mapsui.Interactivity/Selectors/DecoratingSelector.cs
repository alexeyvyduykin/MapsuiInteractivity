using Mapsui.Layers;
using Mapsui.Nts;

namespace Mapsui.Interactivity
{
    public class DecoratingSelector : BaseSelector, IDecoratingSelector
    {
        private IDecorator? _decorator;
        private readonly Func<GeometryFeature, IDecorator> _builder;
        private readonly LayerCollection _layers;

        public DecoratingSelector(LayerCollection layers, Func<GeometryFeature, IDecorator> builder)
        {
            _layers = layers;
            _builder = builder;

            Select.Subscribe(s =>
            {
                if (s.SelectedFeature is GeometryFeature gf)
                {
                    _decorator = _builder.Invoke(gf);

                    _decorator.Cancel += (s, e) => base.Unselected();

                    AddInteractiveLayer(_layers, _decorator);

                    SelectedDecorator?.Invoke(Decorator, EventArgs.Empty);
                }
            });

            Unselect.Subscribe(_ => RemoveInteractiveLayer(_layers));
        }

        private static void AddInteractiveLayer(LayerCollection layers, IInteractive interactive)
        {
            var interactiveLayer = new InteractiveLayer(interactive)
            {
                Name = nameof(InteractiveLayer),
                Style = InteractiveFactory.CreateInteractiveLayerDecoratorStyle(),
            };

            layers.Add(interactiveLayer);
        }

        private static void RemoveInteractiveLayer(LayerCollection layers)
        {
            var interactiveLayer = layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                layers.Remove(interactiveLayer);
            }
        }

        public event EventHandler? SelectedDecorator;

        public IDecorator? Decorator => _decorator;
    }
}
