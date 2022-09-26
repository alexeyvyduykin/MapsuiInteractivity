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

        public GeometryFeature FeatureSource => _featureSource;

        public override void Starting(MapInfo? mapInfo, double screenDistance)
        {
            var vertices = GetActiveVertices();

            var worldPosition = mapInfo?.WorldPosition;

            if (worldPosition != null)
            {
                var vertexTouched = vertices.OrderBy(v => v.Distance(worldPosition)).FirstOrDefault(v => v.Distance(worldPosition) < screenDistance);

                if (vertexTouched != null)
                {
                    Starting(mapInfo);
                }
            }
        }
    }
}
