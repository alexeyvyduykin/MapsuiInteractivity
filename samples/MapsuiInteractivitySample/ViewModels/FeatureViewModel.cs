using Mapsui;
using System;
using System.Linq;

namespace MapsuiInteractivitySample.ViewModels
{
    public class FeatureViewModel : ViewModelBase
    {
        private readonly IFeature _feature;

        public FeatureViewModel(IFeature feature)
        {
            _feature = feature;

            Name = feature.Fields.Contains("Name") ? (string)feature["Name"]! : "Unknown";
        }

        public string Name { get; set; }
    }
}
