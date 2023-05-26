using Mapsui.Nts;
using ReactiveUI;
using System.Reactive.Linq;

namespace Mapsui.Interactivity;

internal class DecoratorSelector : Selector, IDecoratorSelector
{
    private IDecorator? _decorator;

    internal DecoratorSelector(Func<GeometryFeature, IDecorator> builder)
    {
        DecoratorSelecting = ReactiveCommand.Create<IDecorator, IDecorator>(s =>
        {
            _decorator = s;
            return _decorator;
        });

        Select.Subscribe(s =>
        {
            if (s.SelectedFeature is GeometryFeature gf)
            {
                var decorator = builder.Invoke(gf);

                decorator.Canceling.Subscribe(_ => base.Unselected());

                DecoratorSelecting.Execute(decorator).Subscribe();
            }
        });
    }

    public ReactiveCommand<IDecorator, IDecorator> DecoratorSelecting { get; }

    public override void Unselected()
    {
        _decorator?.Cancel();
        _decorator = null;

        base.Unselected();
    }
}
