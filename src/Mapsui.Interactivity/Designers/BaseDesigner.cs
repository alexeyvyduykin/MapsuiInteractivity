using Mapsui.Extensions;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mapsui.Interactivity;

public abstract class BaseDesigner : BaseInteractive, IDesigner
{
    private readonly Subject<IDesigner> _beginCreatingSubj = new();
    private readonly Subject<IDesigner> _creatingSubj = new();
    private readonly Subject<IDesigner> _hoverCreatingSubj = new();
    private readonly Subject<IFeature> _endCreatingSubj = new();

    public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

    public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

    public IObservable<IDesigner> BeginCreating => _beginCreatingSubj.AsObservable();

    public IObservable<IDesigner> Creating => _creatingSubj.AsObservable();

    public IObservable<IDesigner> HoverCreating => _hoverCreatingSubj.AsObservable();

    public IObservable<IFeature> EndCreating => _endCreatingSubj.AsObservable();

    public override IEnumerable<IFeature> GetFeatures()
    {
        var list = new List<IFeature>() { Feature };

        if (ExtraFeatures.Count != 0)
        {
            list.AddRange(ExtraFeatures);
        }

        foreach (var point in GetActiveVertices())
        {
            list.Add(new GeometryFeature { Geometry = point.ToPoint() });
        }

        return list;
    }

    protected void OnBeginCreating()
    {
        _beginCreatingSubj.OnNext(this);
    }

    protected void OnCreating()
    {
        _creatingSubj.OnNext(this);
    }

    protected void OnHoverCreating()
    {
        _hoverCreatingSubj.OnNext(this);
    }

    protected void OnEndCreating()
    {
        _endCreatingSubj.OnNext(Feature.Copy());
    }
}
