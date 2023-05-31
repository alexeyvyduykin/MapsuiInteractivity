using Mapsui.Nts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mapsui.Interactivity;

internal class DecoratorSelector : Selector, IDecoratorSelector
{
    private IDecorator? _decorator;
    private readonly Subject<IDecorator> _decoratorSelectingSubj = new();
    private readonly Subject<GeometryFeature?> _decoratorUnselectingSubj = new();

    internal DecoratorSelector(Func<GeometryFeature, IDecorator> builder)
    {
        Select.Subscribe(s =>
        {
            if (s.Item1 is GeometryFeature gf)
            {
                _decorator = builder.Invoke(gf);

                _decorator.Canceling.Subscribe(_ => base.Unselected());

                _decoratorSelectingSubj.OnNext(_decorator);
            }
        });

        Unselect.Subscribe(s =>
        {
            var feature = _decorator!.IsFeatureChange ? _decorator!.FeatureSource : null;

            _decoratorUnselectingSubj.OnNext(feature);
        });
    }

    public IObservable<IDecorator> DecoratorSelecting => _decoratorSelectingSubj.AsObservable();

    public IObservable<GeometryFeature?> DecoratorUnselecting => _decoratorUnselectingSubj.AsObservable();

    public override void Unselected()
    {
        _decorator?.Cancel();
        _decorator = null;

        base.Unselected();
    }
}
