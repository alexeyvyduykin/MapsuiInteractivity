using Mapsui.Layers;
using System.Reactive.Linq;

namespace Mapsui.Interactivity;

internal class InteractiveLayer : WritableLayer
{
    private readonly IDisposable _disposable;

    public InteractiveLayer(IInteractive interactive)
    {
        _disposable = interactive.Invalidate.Subscribe(UpdateData);

        IsMapInfoLayer = true;

        UpdateData(interactive);
    }

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

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose();
    }
}