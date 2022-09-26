using Mapsui.Nts;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity
{
    public abstract class BaseDesigner : BaseInteractive, IDesigner
    {
        public BaseDesigner()
        {
            HoverCreating = ReactiveCommand.Create<Unit, IDesigner>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        }

        public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

        public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

        public event EventHandler? BeginCreating;

        public event EventHandler? Creating;

        public ReactiveCommand<Unit, IDesigner> HoverCreating { get; }


        public event EventHandler? EndCreating;

        protected void BeginCreatingCallback()
        {
            BeginCreating?.Invoke(this, EventArgs.Empty);
        }

        protected void CreatingCallback()
        {
            Creating?.Invoke(this, EventArgs.Empty);
        }

        protected void EndCreatingCallback()
        {
            EndCreating?.Invoke(this, EventArgs.Empty);
        }
    }
}
