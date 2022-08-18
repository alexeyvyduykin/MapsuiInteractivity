using Mapsui;
using Mapsui.Layers;
using Mapsui.UI;
using System;

namespace Mapsui.Interactivity
{
    public interface ISelectDecorator : IDisposable
    {
        BaseFeature? SelectedFeature { get; }

        void SelectFeature(BaseFeature feature);

        event EventHandler? Select;

        event EventHandler? Unselect;
    }

    internal class SelectDecorator : ISelectDecorator
    {
        private readonly Map _map;
        private readonly ILayer _layer;
        private BaseFeature? _saveFeature;

        public SelectDecorator(Map map, ILayer layer)
        {
            _map = map;
            _layer = layer;

            map.Info += Map_Info;
        }

        public BaseFeature? SelectedFeature => _saveFeature;

        public event EventHandler? Select;

        public event EventHandler? Unselect;

        public void SelectFeature(BaseFeature feature)
        {
            if (SelectedFeature == feature)
            {
                return;
            }

            if (SelectedFeature != null)
            {
                _saveFeature = null;

                UnselectImpl();
            }

            _saveFeature = feature;

            SelectImpl((IFeature)feature);
        }

        private void Map_Info(object? sender, MapInfoEventArgs e)
        {
            if (e.MapInfo != null && e.MapInfo.Layer == _layer && e.MapInfo.Feature != null)
            {
                var feature = e.MapInfo.Feature;

                if (feature != _saveFeature)
                {
                    if (_saveFeature != null)
                    {
                        _saveFeature = null;

                        //TODO: SelectFeature is null, what a feature unselect?
                        UnselectImpl();
                    }

                    _saveFeature = (BaseFeature)feature;

                    SelectImpl(feature);
                }
                else
                {
                    if (_saveFeature != null)
                    {
                        _saveFeature = null;

                        UnselectImpl();
                    }
                }

                return;
            }
        }

        protected virtual void SelectImpl(IFeature feature)
        {
            OnSelect();
        }

        protected void OnSelect()
        {
            Select?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void UnselectImpl()
        {
            OnUnselect();
        }

        private void OnUnselect()
        {
            Unselect?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnselectImpl();

                //HACK: call feature style changing, if select after dispose
                _layer.DataHasChanged();

                _map.Info -= Map_Info;
            }
        }
    }
}
