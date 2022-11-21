using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;

namespace Mapsui.Interactivity;

public class InteractiveLayer : BaseLayer
{
    private readonly IInteractive? _interactive;
    private readonly IDisposable _disposable;
    private bool _cancel;

    public InteractiveLayer(IInteractive interactive)
    {
        _interactive = interactive;

        _disposable = _interactive.Invalidate.Subscribe(_ => DataHasChanged());

        IsMapInfoLayer = true;
    }

    public void Cancel()
    {
        _cancel = true;

        DataHasChanged();
    }

    public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
    {
        if (_cancel == true)
        {
            yield break;
        }

        if (_interactive is IDecorator decorator)
        {
            var feature = decorator.FeatureSource;

            if (feature == null)
            {
                yield break;
            }

            if (box.Intersects(feature.Extent) == true)
            {
                foreach (var point in decorator.GetActiveVertices())
                {
                    yield return new GeometryFeature { Geometry = point.ToPoint() };
                }
            }
        }
        else if (_interactive is IDesigner designer)
        {
            var feature = designer.Feature;

            yield return feature;

            if (designer.ExtraFeatures.Count != 0)
            {
                foreach (var item in designer.ExtraFeatures)
                {
                    yield return item;
                }
            }

            foreach (var point in designer.GetActiveVertices())
            {
                yield return new GeometryFeature { Geometry = point.ToPoint() };
            }
        }
    }

    // TODO: Mapsui 4.0.0-beta.3 > Mapsui 4.0.0-beta.5

    //public override void RefreshData(FetchInfo fetchInfo)
    //{
    //    OnDataChanged(new DataChangedEventArgs());
    //}

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose();
    }
}
