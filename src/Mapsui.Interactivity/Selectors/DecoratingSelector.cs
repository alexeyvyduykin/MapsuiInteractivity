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

            Select += DecoratingSelector_Select;
            Unselect += DecoratingSelector_Unselect;
        }

        private void DecoratingSelector_Unselect(object? sender, EventArgs e)
        {
            RemoveInteractiveLayer(_layers);
        }

        private void DecoratingSelector_Select(object? sender, EventArgs e)
        {
            if (sender is GeometryFeature gf)
            {
                _decorator = _builder.Invoke(gf);

                _decorator.Cancel += (s, e) => base.Unselected();

                AddInteractiveLayer(_layers, _decorator);

                SelectedDecorator?.Invoke(Decorator, EventArgs.Empty);
            }
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
