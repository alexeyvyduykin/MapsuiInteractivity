using Mapsui.Layers;
using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class BaseSelector : BaseInteractive, ISelector
    {
        private IFeature? _lastSelectedFeature;
        private IFeature? _lastPointeroverFeature;
        private ILayer? _lastPointeroverLayer;

        public event EventHandler? Select;
        public event EventHandler? Unselect;
        public event EventHandler? HoveringBegin;
        public event EventHandler? HoveringEnd;

        public void Selected(IFeature feature, ILayer layer)
        {
            if (layer is WritableLayer wl)
            {
                if (_lastSelectedFeature != null)
                {
                    _lastSelectedFeature["selected"] = false;

                    Unselect?.Invoke(_lastSelectedFeature, EventArgs.Empty);
                }

                if (feature != null)
                {
                    feature["selected"] = true;

                    _lastSelectedFeature = feature;
                }

                layer.DataHasChanged();

                Select?.Invoke(_lastSelectedFeature, EventArgs.Empty);
            }
        }

        public void Unselected()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature["pointerover"] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoveringEnd?.Invoke(_lastPointeroverFeature, EventArgs.Empty);
            }

            if (_lastSelectedFeature != null)
            {
                _lastSelectedFeature["selected"] = false;

                _lastPointeroverLayer?.DataHasChanged();

                Unselect?.Invoke(_lastSelectedFeature, EventArgs.Empty);
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

                        Unselect?.Invoke(_lastSelectedFeature, EventArgs.Empty);
                    }

                    _lastSelectedFeature = feature;

                    _lastSelectedFeature["selected"] = true;

                    Select?.Invoke(_lastSelectedFeature, EventArgs.Empty);
                }
                else if (_lastSelectedFeature != null && feature == _lastSelectedFeature)
                {
                    if (_lastSelectedFeature.Fields.Contains("selected"))
                    {
                        var isSelected = !(bool)_lastSelectedFeature["selected"]!;
                        _lastSelectedFeature["selected"] = isSelected;

                        if (isSelected == true)
                        {
                            Select?.Invoke(_lastSelectedFeature, EventArgs.Empty);
                        }
                        else
                        {
                            Unselect?.Invoke(_lastSelectedFeature, EventArgs.Empty);
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

            HoveringBegin?.Invoke(_lastPointeroverFeature, EventArgs.Empty);
        }

        public override void PointeroverStop()
        {
            if (_lastPointeroverFeature != null)
            {
                _lastPointeroverFeature["pointerover"] = false;

                _lastPointeroverLayer?.DataHasChanged();

                HoveringEnd?.Invoke(_lastPointeroverFeature, EventArgs.Empty);
            }
        }
    }
}
