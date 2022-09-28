using Mapsui.Layers;
using Mapsui.UI;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public class Selector : BaseInteractive, ISelector
    {
        private IFeature? _lastSelectedFeature;
        private IFeature? _lastPointeroverFeature;
        private ILayer? _lastPointeroverLayer;

        internal Selector() : base()
        {
            Select = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            Unselect = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            HoverBegin = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            HoverEnd = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<Unit, ISelector> Select { get; }

        public ReactiveCommand<Unit, ISelector> Unselect { get; }

        public ReactiveCommand<Unit, ISelector> HoverBegin { get; }

        public ReactiveCommand<Unit, ISelector> HoverEnd { get; }

        public IFeature? SelectedFeature => _lastSelectedFeature;

        public IFeature? HoveringFeature => _lastPointeroverFeature;

        public void Selected(IFeature feature, ILayer layer)
        {
            if (layer is WritableLayer)
            {
                if (_lastSelectedFeature != null)
                {
                    _lastSelectedFeature[InteractiveFields.Select] = false;

                    Unselect?.Execute().Subscribe();
                }

                feature[InteractiveFields.Select] = true;

                _lastSelectedFeature = feature;

                layer.DataHasChanged();

                Select?.Execute().Subscribe();
            }
        }

        public virtual void Unselected()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature[InteractiveFields.Hover] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoverEnd?.Execute().Subscribe();
            }

            if (_lastSelectedFeature != null)
            {
                _lastSelectedFeature[InteractiveFields.Select] = false;

                _lastPointeroverLayer?.DataHasChanged();

                Unselect?.Execute().Subscribe();
            }
        }

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            if (mapInfo != null
                && mapInfo.Layer != null
                && mapInfo.Feature != null)
            {
                var feature = mapInfo.Feature;

                if (feature != _lastSelectedFeature)
                {
                    if (_lastSelectedFeature != null)
                    {
                        _lastSelectedFeature[InteractiveFields.Select] = false;

                        Unselect?.Execute().Subscribe();
                    }

                    feature[InteractiveFields.Select] = true;

                    _lastSelectedFeature = feature;

                    Select?.Execute().Subscribe();
                }
                else if (_lastSelectedFeature != null && feature == _lastSelectedFeature)
                {
                    if (_lastSelectedFeature.Fields.Contains(InteractiveFields.Select))
                    {
                        var isSelected = !(bool)_lastSelectedFeature[InteractiveFields.Select]!;

                        _lastSelectedFeature[InteractiveFields.Select] = isSelected;

                        if (isSelected == true)
                        {
                            Select?.Execute().Subscribe();
                        }
                        else
                        {
                            Unselect?.Execute().Subscribe();
                        }
                    }
                }

                mapInfo.Layer?.DataHasChanged();
            }
        }

        public override IEnumerable<MPoint> GetActiveVertices() => new List<MPoint>();

        public override void HoveringBegin(MapInfo? mapInfo)
        {
            if (mapInfo != null
                && mapInfo.Feature != null
                && mapInfo.Layer != null)
            {
                HoveringBegin(mapInfo.Feature, mapInfo.Layer);
            }
        }

        public void HoveringBegin(IFeature feature, ILayer layer)
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature[InteractiveFields.Hover] = false;
            }

            feature[InteractiveFields.Hover] = true;

            layer.DataHasChanged();

            _lastPointeroverFeature = feature;
            _lastPointeroverLayer = layer;

            HoverBegin?.Execute().Subscribe();
        }

        public override void HoveringEnd()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature[InteractiveFields.Hover] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoverEnd?.Execute().Subscribe();
            }
        }
    }
}
