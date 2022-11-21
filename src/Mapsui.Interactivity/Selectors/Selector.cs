using Mapsui.Layers;
using Mapsui.UI;
using ReactiveUI;

namespace Mapsui.Interactivity;

public class Selector : BaseInteractive, ISelector
{
    private IFeature? _lastSelectedFeature;
    private ILayer? _lastSelectedLayer;
    private IFeature? _lastPointeroverFeature;
    private ILayer? _lastPointeroverLayer;
    private bool _selectState = false;

    internal Selector() : base()
    {
        Select = ReactiveCommand.Create<ISelector, ISelector>(s => s, outputScheduler: RxApp.MainThreadScheduler);
        Unselect = ReactiveCommand.Create<ISelector, ISelector>(s => s, outputScheduler: RxApp.MainThreadScheduler);
        HoverBegin = ReactiveCommand.Create<ISelector, ISelector>(s => s, outputScheduler: RxApp.MainThreadScheduler);
        HoverEnd = ReactiveCommand.Create<ISelector, ISelector>(s => s, outputScheduler: RxApp.MainThreadScheduler);
    }

    public ReactiveCommand<ISelector, ISelector> Select { get; }

    public ReactiveCommand<ISelector, ISelector> Unselect { get; }

    public ReactiveCommand<ISelector, ISelector> HoverBegin { get; }

    public ReactiveCommand<ISelector, ISelector> HoverEnd { get; }

    public IFeature? SelectedFeature => _lastSelectedFeature;

    public ILayer? SelectedLayer => _lastSelectedLayer;

    public IFeature? HoveringFeature => _lastPointeroverFeature;

    public ILayer? PointeroverLayer => _lastPointeroverLayer;

    public void Selected(IFeature feature, ILayer layer)
    {
        if (_lastSelectedFeature != null)
        {
            Unselect?.Execute(this).Subscribe();
        }

        _lastSelectedFeature = feature;
        _lastSelectedLayer = layer;

        _selectState = true;

        Select?.Execute(this).Subscribe();
    }

    public virtual void Unselected()
    {
        if (_lastPointeroverFeature != null)
        {
            HoverEnd?.Execute(this).Subscribe();
        }

        if (_lastSelectedFeature != null)
        {
            Unselect?.Execute(this).Subscribe();
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
                    Unselect?.Execute(this).Subscribe();
                }

                _lastSelectedFeature = feature;
                _lastSelectedLayer = layer;

                _selectState = true;

                Select?.Execute(this).Subscribe();
            }
            else if (_lastSelectedFeature != null && feature == _lastSelectedFeature)
            {
                var isSelected = _selectState;

                if (isSelected == true)
                {
                    Unselect?.Execute(this).Subscribe();
                }
                else
                {
                    Select?.Execute(this).Subscribe();
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

        HoverBegin?.Execute(this).Subscribe();
    }

    public override void HoveringEnd()
    {
        HoverEnd?.Execute(this).Subscribe();
    }

    public override void Cancel()
    {
        Unselected();

        base.Cancel();
    }
}
