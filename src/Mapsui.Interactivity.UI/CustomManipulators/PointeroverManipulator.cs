using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI;

namespace Mapsui.Interactivity.UI
{
    internal class PointeroverManipulator : MouseManipulator
    {
        private bool _isChecker = false;
        private IFeature? _lastFeature;
        private MapInfo? _lastMapInfo;

        public PointeroverManipulator(IView view) : base(view) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (e.Handled == false)
            {
                var mapInfo = e.MapInfo;

                if (mapInfo != null
                    && mapInfo.Layer != null
                    && mapInfo.Feature != null)
                {
                    if (_lastFeature != null && _lastFeature != mapInfo.Feature)
                    {
                        if (_lastMapInfo != null)
                        {
                            View.Interactive.PointeroverStop();
                        }

                        _lastFeature = mapInfo.Feature;

                        if (_lastFeature != null)
                        {
                            View.Interactive.PointeroverStart(mapInfo);
                            _lastMapInfo = mapInfo;
                        }
                    }

                    if (_isChecker == true)
                    {
                        _lastFeature = mapInfo.Feature;

                        View.SetCursor(CursorType.Hand);

                        if (_lastFeature != null)
                        {
                            View.Interactive.PointeroverStart(mapInfo);
                            _lastMapInfo = mapInfo;
                        }

                        _isChecker = false;
                    }

                    e.Handled = false;// true;
                }
                else
                {
                    if (_isChecker == false)
                    {
                        View.SetCursor(CursorType.Default);

                        if (_lastFeature != null)
                        {
                            View.Interactive.PointeroverStop();
                        }

                        _isChecker = true;
                    }
                }
            }
        }
    }
}
