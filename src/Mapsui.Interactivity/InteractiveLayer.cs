using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace Mapsui.Interactivity;

internal class InteractiveLayer : BaseLayer, IModifyFeatureLayer
{
    private readonly ConcurrentQueue<IFeature> _cache = new();
    private readonly IDisposable _disposable;

    public InteractiveLayer(IInteractive interactive)
    {
        _disposable = interactive.Invalidate.Subscribe(UpdateData);

        IsMapInfoLayer = true;

        UpdateData(interactive);
    }

    public override MRect? Extent => GetExtent();

    private void UpdateData(IInteractive interactive)
    {
        Clear();

        AddRange(interactive.GetFeatures());

        DataHasChanged();
    }

    public void Cancel()
    {
        Clear();

        DataHasChanged();
    }

    public IEnumerable<IFeature> GetFeatures()
    {
        return _cache;
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Add(IFeature feature)
    {
        _cache.Enqueue(feature);
    }

    public void AddRange(IEnumerable<IFeature> features)
    {
        foreach (IFeature feature in features)
        {
            _cache.Enqueue(feature);
        }
    }

    public IFeature? Find(IFeature feature)
    {
        IFeature feature2 = feature;
        return _cache.FirstOrDefault((IFeature f) => f == feature2);
    }

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose();
    }

    public override IEnumerable<IFeature> GetFeatures(MRect? box, double resolution)
    {
        if (box == null)
        {
            return new List<IFeature>();
        }

        ConcurrentQueue<IFeature> cache = _cache;
        MRect biggerBox = box!.Grow(SymbolStyle.DefaultWidth * 2.0 * resolution, SymbolStyle.DefaultHeight * 2.0 * resolution);
        return cache.Where((IFeature f) => biggerBox.Intersects(f.Extent));
    }

    private MRect? GetExtent()
    {
        List<MRect> list = (from f in _cache
                            select f.Extent into g
                            where g != null
                            select g).ToList();
        if (list.Count == 0)
        {
            return null;
        }

        double minX = list.Min((MRect g) => g.MinX);
        double minY = list.Min((MRect g) => g.MinY);
        double maxX = list.Max((MRect g) => g.MaxX);
        double maxY = list.Max((MRect g) => g.MaxY);
        return new MRect(minX, minY, maxX, maxY);
    }
}