using Mapsui.Nts;
using Mapsui.UI;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public abstract class BaseDecorator : BaseInteractive, IDecorator
    {
        private readonly GeometryFeature _featureSource;

        public BaseDecorator(GeometryFeature featureSource)
        {
            _featureSource = featureSource;
        }

        protected void UpdateGeometry(Geometry geometry)
        {
            _featureSource.Geometry = geometry;

            _featureSource.RenderedGeometry.Clear();

            Invalidate();
        }

        public override void Hovering(MapInfo? mapInfo)
        {

        }

        public GeometryFeature FeatureSource => _featureSource;
    }
}
