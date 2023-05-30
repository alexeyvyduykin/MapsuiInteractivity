using Mapsui.Layers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mapsui.Interactivity;

public class Selector : BaseInteractive, ISelector
{
    private IFeature? _lastSelectedFeature;
    private ILayer? _lastSelectedLayer;
    private IFeature? _lastPointeroverFeature;
    private ILayer? _lastPointeroverLayer;
    private bool _selectState = false;
    private readonly Subject<(IFeature, ILayer)> _selectSubj = new();
    private readonly Subject<(IFeature, ILayer)> _unselectSubj = new();
    private readonly Subject<(IFeature, ILayer)> _hoverBeginSubj = new();
    private readonly Subject<(IFeature, ILayer)> _hoverEndSubj = new();

    public IObservable<(IFeature, ILayer)> Select => _selectSubj.AsObservable();

    public IObservable<(IFeature, ILayer)> Unselect => _unselectSubj.AsObservable();

    public IObservable<(IFeature, ILayer)> HoverBegin => _hoverBeginSubj.AsObservable();

    public IObservable<(IFeature, ILayer)> HoverEnd => _hoverEndSubj.AsObservable();

    public void Selected(IFeature feature, ILayer layer)
    {
        if (_lastSelectedFeature is { } && _lastSelectedLayer is { })
        {
            _unselectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
        }

        _lastSelectedFeature = feature;
        _lastSelectedLayer = layer;

        _selectState = true;

        _selectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
    }

    public virtual void Unselected()
    {
        HoveringEnd();

        if (_lastSelectedFeature is { } && _lastSelectedLayer is { })
        {
            _unselectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
        }

        _selectState = false;
    }

    public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
    {
        if (mapInfo?.Layer is ILayer layer
            && mapInfo?.Feature is IFeature feature)
        {
            if (feature != _lastSelectedFeature)
            {
                if (_lastSelectedFeature is { } && _lastSelectedLayer is { })
                {
                    _unselectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
                }

                _lastSelectedFeature = feature;
                _lastSelectedLayer = layer;

                _selectState = true;

                _selectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
            }
            else if (_lastSelectedFeature is { } && _lastSelectedLayer is { } && feature == _lastSelectedFeature)
            {
                var isSelected = _selectState;

                if (isSelected == true)
                {
                    _unselectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
                }
                else
                {
                    _selectSubj.OnNext((_lastSelectedFeature, _lastSelectedLayer));
                }

                _selectState = !_selectState;
            }
        }
    }

    public override IEnumerable<MPoint> GetActiveVertices() => new List<MPoint>();

    public override IEnumerable<IFeature> GetFeatures() => new List<IFeature>();

    public override void HoveringBegin(MapInfo? mapInfo)
    {
        if (mapInfo?.Feature is IFeature feature
            && mapInfo?.Layer is ILayer layer)
        {
            HoveringBegin(feature, layer);
        }
    }

    public void HoveringBegin(IFeature feature, ILayer layer)
    {
        HoveringEnd();

        _lastPointeroverFeature = feature;
        _lastPointeroverLayer = layer;

        _hoverBeginSubj.OnNext((feature, layer));
    }

    public override void HoveringEnd()
    {
        if (_lastPointeroverFeature is { } && _lastPointeroverLayer is { })
        {
            _hoverEndSubj.OnNext((_lastPointeroverFeature, _lastPointeroverLayer));
        }
    }

    public override void Cancel()
    {
        Unselected();

        base.Cancel();
    }
}
