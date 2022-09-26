using Mapsui.Layers;
using Mapsui.UI;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public class BaseSelector : BaseInteractive, ISelector
    {
        private IFeature? _lastSelectedFeature;
        private IFeature? _lastPointeroverFeature;
        private ILayer? _lastPointeroverLayer;

        public BaseSelector() : base()
        {
            Select = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            Unselect = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            HoveringBegin = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
            HoveringEnd = ReactiveCommand.Create<Unit, ISelector>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<Unit, ISelector> Select { get; }

        public ReactiveCommand<Unit, ISelector> Unselect { get; }

        public ReactiveCommand<Unit, ISelector> HoveringBegin { get; }

        public ReactiveCommand<Unit, ISelector> HoveringEnd { get; }

        public IFeature? SelectedFeature => _lastSelectedFeature;

        public IFeature? HoveringFeature => _lastPointeroverFeature;

        public void Selected(IFeature feature, ILayer layer)
        {
            if (layer is WritableLayer wl)
            {
                if (_lastSelectedFeature != null)
                {
                    _lastSelectedFeature["selected"] = false;

                    Unselect?.Execute().Subscribe();
                }

                feature["selected"] = true;

                _lastSelectedFeature = feature;

                layer.DataHasChanged();

                Select?.Execute().Subscribe();
            }
        }

        public void Unselected()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature["pointerover"] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoveringEnd?.Execute().Subscribe();
            }

            if (_lastSelectedFeature != null)
            {
                _lastSelectedFeature["selected"] = false;

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
                        _lastSelectedFeature["selected"] = false;

                        Unselect?.Execute().Subscribe();
                    }

                    feature["selected"] = true;

                    _lastSelectedFeature = feature;

                    Select?.Execute().Subscribe();
                }
                else if (_lastSelectedFeature != null && feature == _lastSelectedFeature)
                {
                    if (_lastSelectedFeature.Fields.Contains("selected"))
                    {
                        var isSelected = !(bool)_lastSelectedFeature["selected"]!;

                        _lastSelectedFeature["selected"] = isSelected;

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

        public override void PointeroverStart(MapInfo? mapInfo)
        {
            if (mapInfo != null
                && mapInfo.Feature != null
                && mapInfo.Layer != null)
            {
                PointeroverStart(mapInfo.Feature, mapInfo.Layer);
            }
        }

        public void PointeroverStart(IFeature feature, ILayer layer)
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature["pointerover"] = false;
            }

            feature["pointerover"] = true;

            layer.DataHasChanged();

            _lastPointeroverFeature = feature;
            _lastPointeroverLayer = layer;

            HoveringBegin?.Execute().Subscribe();
        }

        public override void PointeroverStop()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature["pointerover"] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoveringEnd?.Execute().Subscribe();
            }
        }
    }
}
