using Mapsui.Nts;
using ReactiveUI;
using System.Reactive;

namespace Mapsui.Interactivity;

public abstract class BaseDesigner : BaseInteractive, IDesigner
{
    public BaseDesigner()
    {
        BeginCreating = ReactiveCommand.Create<Unit, IDesigner>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        Creating = ReactiveCommand.Create<Unit, IDesigner>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        HoverCreating = ReactiveCommand.Create<Unit, IDesigner>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
        EndCreating = ReactiveCommand.Create<Unit, IDesigner>(_ => this, outputScheduler: RxApp.MainThreadScheduler);
    }

    public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

    public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

    public ReactiveCommand<Unit, IDesigner> BeginCreating { get; }

    public ReactiveCommand<Unit, IDesigner> Creating { get; }

    public ReactiveCommand<Unit, IDesigner> HoverCreating { get; }

    public ReactiveCommand<Unit, IDesigner> EndCreating { get; }
}
