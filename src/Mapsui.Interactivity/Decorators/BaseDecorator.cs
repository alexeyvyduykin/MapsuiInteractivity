using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity;

public abstract class BaseDecorator : BaseInteractive, IDecorator
{
    private readonly GeometryFeature _featureSource;

    public BaseDecorator(GeometryFeature featureSource)
    {
        _featureSource = featureSource;

        OnInvalidate();
    }

    public GeometryFeature FeatureSource => _featureSource;

    public bool IsFeatureChange { get; private set; } = false;

    public override IEnumerable<IFeature> GetFeatures()
    {
        return GetActiveVertices()
            .Select(s => new GeometryFeature { Geometry = s.ToPoint() });
    }

    protected void UpdateGeometry(Geometry geometry)
    {
        IsFeatureChange = true;

        _featureSource.Geometry = geometry;

        _featureSource.RenderedGeometry.Clear();

        OnInvalidate();
    }

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
