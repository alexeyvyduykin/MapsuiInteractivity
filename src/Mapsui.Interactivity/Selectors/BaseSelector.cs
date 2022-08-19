using Mapsui.Layers;
using Mapsui.UI;
using System.Diagnostics;

namespace Mapsui.Interactivity
{
    public class BaseSelector : BaseInteractive, ISelector
    {
        private IFeature? _selectedFeature;
        private IFeature? _lastFeature;
        private ILayer? _lastLayer;
        private bool _isChecker;

        public event EventHandler? Selecting;
        public event EventHandler? Unselecting;

        public void Click(MapInfo? mapInfo)
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

                    Selecting?.Invoke(_selectedFeature, EventArgs.Empty);
                }
                else if (_selectedFeature != null && feature == _selectedFeature)
                {
                    if (_selectedFeature.Fields.Contains("selected"))
                    {
                        var isSelected = !(bool)_selectedFeature["selected"]!;
                        _selectedFeature["selected"] = isSelected;

                        if (isSelected == true)
                        {
                            Selecting?.Invoke(_selectedFeature, EventArgs.Empty);
                        }
                        else
                        {
                            Unselecting?.Invoke(_selectedFeature, EventArgs.Empty);
                        }
                    }
                }

                mapInfo.Layer?.DataHasChanged();
            }
        }

        public void Pointerover(MapInfo? mapInfo)
        {
            if (mapInfo != null
                && mapInfo.Layer != null
                && mapInfo.Feature != null
                )
            {
                if (_lastFeature != null && _lastFeature != mapInfo.Feature)
                {
                    _lastFeature["pointerover"] = false;
                    _lastFeature = mapInfo.Feature;
                    _lastLayer = mapInfo.Layer;

                    if (_lastFeature != null)
                    {
                        _lastFeature["pointerover"] = true;
                    }

                    mapInfo.Layer?.DataHasChanged();
                }

                if (_isChecker == true)
                {
                    _lastFeature = mapInfo.Feature;
                    _lastLayer = mapInfo.Layer;

                    if (_lastFeature != null)
                    {
                        _lastFeature["pointerover"] = true;

                        mapInfo.Layer?.DataHasChanged();
                    }

                    _isChecker = false;
                }

            }
            else
            {
                if (_isChecker == false)
                {

                    if (_lastFeature != null)
                    {
                        _lastFeature["pointerover"] = false;

                        _lastLayer?.DataHasChanged();
                    }

                    _isChecker = true;
                }
            }
        }

        public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {

        }

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            throw new NotImplementedException();
        }

        public override void Hovering(MPoint worldPosition)
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

                Debug.WriteLine("pointerover = true");

                mapInfo?.Layer?.DataHasChanged();
            }
        }

        public void PointeroverStop(MapInfo? mapInfo)
        {
            if (_lastFeature != null)
            {
                _lastFeature["pointerover"] = false;

                Debug.WriteLine("pointerover = false");

                _lastLayer?.DataHasChanged();

                mapInfo?.Layer?.DataHasChanged();
            }
        }
    }
}
