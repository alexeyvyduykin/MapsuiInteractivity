using Mapsui.Layers;
using Mapsui.Nts;
using ReactiveUI;
using System.Reactive;

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

            DecoratorSelecting = ReactiveCommand.Create<Unit, IDecorator>(_ => _decorator!);

            Select.Subscribe(s =>
            {
                if (s.SelectedFeature is GeometryFeature gf)
                {
                    _decorator = _builder.Invoke(gf);

                    _decorator.Canceling.Subscribe(_ => base.Unselected());

                    AddInteractiveLayer(_layers, _decorator);

                    DecoratorSelecting?.Execute().Subscribe();
                }
            });

            Unselect.Subscribe(_ => RemoveInteractiveLayer(_layers));
        }

        public ReactiveCommand<Unit, IDecorator> DecoratorSelecting { get; }

        public override void Unselected()
        {
            _decorator?.Canceling.Execute().Subscribe();

            _decorator = null;

            base.Unselected();
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
    }
}
