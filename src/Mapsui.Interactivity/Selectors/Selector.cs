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
    private readonly Subject<ISelector> _selectSubj = new();
    private readonly Subject<ISelector> _unselectSubj = new();
    private readonly Subject<ISelector> _hoverBeginSubj = new();
    private readonly Subject<ISelector> _hoverEndSubj = new();

    public IObservable<ISelector> Select => _selectSubj.AsObservable();

    public IObservable<ISelector> Unselect => _unselectSubj.AsObservable();

    public IObservable<ISelector> HoverBegin => _hoverBeginSubj.AsObservable();

    public IObservable<ISelector> HoverEnd => _hoverEndSubj.AsObservable();

    public IFeature? SelectedFeature => _lastSelectedFeature;

    public ILayer? SelectedLayer => _lastSelectedLayer;

    public IFeature? HoveringFeature => _lastPointeroverFeature;

    public ILayer? PointeroverLayer => _lastPointeroverLayer;

    public void Selected(IFeature feature, ILayer layer)
    {
        if (_lastSelectedFeature != null)
        {
            _unselectSubj.OnNext(this);
        }

        _lastSelectedFeature = feature;
        _lastSelectedLayer = layer;

        _selectState = true;

        _selectSubj.OnNext(this);
    }

    public virtual void Unselected()
    {
        if (_lastPointeroverFeature != null)
        {
            _hoverEndSubj.OnNext(this);
        }

        if (_lastSelectedFeature != null)
        {
            _unselectSubj.OnNext(this);
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
                if (_lastSelectedFeature != null)
                {
                    _unselectSubj.OnNext(this);
                }

                _lastSelectedFeature = feature;
                _lastSelectedLayer = layer;

                _selectState = true;

                _selectSubj.OnNext(this);
            }
            else if (_lastSelectedFeature != null && feature == _lastSelectedFeature)
            {
                var isSelected = _selectState;

                if (isSelected == true)
                {
                    _unselectSubj.OnNext(this);
                }
                else
                {
                    _selectSubj.OnNext(this);
                }

                _selectState = !_selectState;
            }
        }
    }

    public override IEnumerable<MPoint> GetActiveVertices() => new List<MPoint>();

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

        _hoverBeginSubj.OnNext(this);
    }

    public override void HoveringEnd()
    {
        _hoverEndSubj.OnNext(this);
    }

    public override void Cancel()
    {
        Unselected();

        base.Cancel();
    }
}
