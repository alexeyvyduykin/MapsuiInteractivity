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
    private readonly Subject<IDesigner> _endCreatingSubj = new();

    public GeometryFeature Feature { get; protected set; } = new GeometryFeature();

    public IList<GeometryFeature> ExtraFeatures { get; protected set; } = new List<GeometryFeature>();

    public IObservable<IDesigner> BeginCreating => _beginCreatingSubj.AsObservable();

    public IObservable<IDesigner> Creating => _creatingSubj.AsObservable();

    public IObservable<IDesigner> HoverCreating => _hoverCreatingSubj.AsObservable();

    public IObservable<IDesigner> EndCreating => _endCreatingSubj.AsObservable();

    public override IEnumerable<IFeature> GetFeatures()
    {
        var feature = Feature;

        yield return feature;

        if (ExtraFeatures.Count != 0)
        {
            foreach (var item in ExtraFeatures)
            {
                yield return item;
            }
        }

        foreach (var point in GetActiveVertices())
        {
            yield return new GeometryFeature { Geometry = point.ToPoint() };
        }
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
        _endCreatingSubj.OnNext(this);
    }
}
