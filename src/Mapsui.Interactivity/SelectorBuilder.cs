using Mapsui.Interactivity.Interfaces;
using Mapsui.Layers;

namespace Mapsui.Interactivity;

internal class SelectorBuilder : ISelectorBuilder
{
    private readonly Func<IInteractive> _builder;
    private IList<string>? _availableLayers;
    private readonly IList<ILayer> _dirtyLayers = new List<ILayer>();

    public SelectorBuilder(Func<IInteractive> builder)
    {
        _builder = builder;
    }

    internal LayerCollection? Layers { get; set; }

    public ISelectorBuilder AvailableFor(ILayer[] layers)
    {
        _availableLayers = layers.Select(s => s.Name).ToList();

        return this;
    }

    public ISelectorBuilder AvailableFor(ILayer layer)
    {
        _availableLayers = new List<string>() { layer.Name };

        return this;
    }

    public ISelectorBuilder AttachTo(LayerCollection layers)
    {
        Layers = layers;

        return this;
    }

    public ISelectorBuilder AttachTo(Map map)
    {
        Layers = map.Layers;

        return this;
    }

    public ISelector Build()
    {
        var selector = (ISelector)_builder.Invoke();

        if (_availableLayers != null && Layers != null)
        {
            foreach (var item in Layers)
            {
                if (_availableLayers.Contains(item.Name) == true)
                {
                    if (item.IsMapInfoLayer == false)
                    {
                        item.IsMapInfoLayer = true;

                        _dirtyLayers.Add(item);
                    }
                }
                else
                {
                    if (item.IsMapInfoLayer == true)
                    {
                        item.IsMapInfoLayer = false;

                        _dirtyLayers.Add(item);
                    }
                }
            }
        }

        selector.Canceling.Subscribe(s =>
        {
            foreach (var item in _dirtyLayers)
            {
                item.IsMapInfoLayer = !item.IsMapInfoLayer;
            }

            _dirtyLayers.Clear();
        });

        return selector;
    }
}
