using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace MapsuiInteractivitySample.ViewModels
{
    public class LayerViewModel : ViewModelBase
    {
        private readonly ILayer _layer;

        public LayerViewModel(ILayer layer)
        {
            _layer = layer;

            Name = layer.Name;

            IsVisible = layer.Enabled;

            IsSelectable = layer.IsMapInfoLayer;

            this.WhenAnyValue(s => s.IsVisible).Skip(1).Subscribe(s =>
            {
                _layer.Enabled = s;
            });

            this.WhenAnyValue(s => s.IsSelectable).Skip(1).Subscribe(s =>
            {
                _layer.IsMapInfoLayer = s;
            });
        }

        public IEnumerable<IFeature> GetFeatures()
        {
            if (_layer is WritableLayer writableLayer)
            {
                return writableLayer.GetFeatures();
            }

            return new List<IFeature>();
        }

        public string Name { get; set; }

        [Reactive]
        public bool IsVisible { get; set; }

        [Reactive]
        public bool IsSelectable { get; set; }
    }
}
