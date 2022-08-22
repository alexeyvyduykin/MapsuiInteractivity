using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public abstract class BaseDecorator : BaseInteractive, IDecorator
    {
        private readonly GeometryFeature _featureSource;

        public event EventHandler? BeginDecorating;

        public event EventHandler? EndDecorating;

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

        protected void BeginDecoratingCallback()
        {
            BeginDecorating?.Invoke(this, EventArgs.Empty);
        }

        protected void EndDecoratingCallback()
        {
            EndDecorating?.Invoke(this, EventArgs.Empty);
        }

        public GeometryFeature FeatureSource => _featureSource;
    }
    //public abstract class BaseDesigner : BaseInteractive, IDesigner
    //{
    //    public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

    //    public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

    //    public event EventHandler? BeginCreating;

    //    public event EventHandler? Creating;

    //    public event EventHandler? HoverCreating;

    //    public event EventHandler? EndCreating;

    //    protected void BeginCreatingCallback()
    //    {
    //        BeginCreating?.Invoke(this, EventArgs.Empty);
    //    }

    //    protected void CreatingCallback()
    //    {
    //        Creating?.Invoke(this, EventArgs.Empty);
    //    }

    //    protected void HoverCreatingCallback()
    //    {
    //        HoverCreating?.Invoke(this, EventArgs.Empty);
    //    }

    //    protected void EndCreatingCallback()
    //    {
    //        EndCreating?.Invoke(this, EventArgs.Empty);
    //    }
    //}
}
