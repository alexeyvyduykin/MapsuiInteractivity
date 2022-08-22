using Mapsui.Nts;

namespace Mapsui.Interactivity
{
    public abstract class BaseDesigner : BaseInteractive, IDesigner
    {
        public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

        public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

        public event EventHandler? BeginCreating;

        public event EventHandler? Creating;

        public event EventHandler? HoverCreating;

        public event EventHandler? EndCreating;

        protected void BeginCreatingCallback()
        {
            BeginCreating?.Invoke(this, EventArgs.Empty);
        }

        protected void CreatingCallback()
        {
            Creating?.Invoke(this, EventArgs.Empty);
        }

        protected void HoverCreatingCallback()
        {
            HoverCreating?.Invoke(this, EventArgs.Empty);
        }

        protected void EndCreatingCallback()
        {
            EndCreating?.Invoke(this, EventArgs.Empty);
        }
    }
}
