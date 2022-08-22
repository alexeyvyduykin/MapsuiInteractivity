using Mapsui.Layers;
using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class BaseSelector : BaseInteractive, ISelector
    {
        private IFeature? _selectedFeature;
        private IFeature? _lastFeature;
        private ILayer? _lastLayer;
        private bool _isChecker;

        public event EventHandler? Select;
        public event EventHandler? Unselect;

        public override void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            if (mapInfo != null
                && mapInfo.Layer != null
                && mapInfo.Feature != null)
            {
                var feature = mapInfo.Feature;

                if (feature != _selectedFeature)
                {
                    if (_selectedFeature != null)
                    {
                        _selectedFeature["selected"] = false;
                    }

                    _selectedFeature = feature;

                    _selectedFeature["selected"] = true;

                    Select?.Invoke(_selectedFeature, EventArgs.Empty);
                }
                else if (_selectedFeature != null && feature == _selectedFeature)
                {
                    if (_selectedFeature.Fields.Contains("selected"))
                    {
                        var isSelected = !(bool)_selectedFeature["selected"]!;
                        _selectedFeature["selected"] = isSelected;

                        if (isSelected == true)
                        {
                            Select?.Invoke(_selectedFeature, EventArgs.Empty);
                        }
                        else
                        {
                            Unselect?.Invoke(_selectedFeature, EventArgs.Empty);
                        }
                    }
                }

                mapInfo.Layer?.DataHasChanged();
            }
        }

        public void Unselected()
        {
            if (_selectedFeature != null)
            {
                _selectedFeature["selected"] = false;

                _lastFeature!["pointerover"] = false;

                _lastLayer?.DataHasChanged();

                Unselect?.Invoke(_selectedFeature, EventArgs.Empty);
            }
        }

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            throw new NotImplementedException();
        }

        public override void Hovering(MapInfo? mapInfo)
        {

        }

        public override void Moving(MPoint worldPosition)
        {

        }

        public override void Starting(MPoint worldPosition)
        {

        }

        public void PointeroverStart(MapInfo? mapInfo)
        {
            _lastFeature = mapInfo?.Feature;
            _lastLayer = mapInfo?.Layer;

            if (_lastFeature != null)
            {
                _lastFeature["pointerover"] = true;

                mapInfo?.Layer?.DataHasChanged();
            }
        }

        public void PointeroverStop(MapInfo? mapInfo)
        {
            if (_lastFeature != null)
            {
                _lastFeature["pointerover"] = false;

                _lastLayer?.DataHasChanged();

                mapInfo?.Layer?.DataHasChanged();
            }
        }
    }
}
