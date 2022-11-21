using Mapsui;
using Mapsui.Layers;
using System;

namespace MapsuiInteractivitySample.ViewModels;

public class FeatureManager
{
    private ILayer? _layer;
    private ILayer? _lastSelectLayer;
    private ILayer? _lastHoverLayer;
    private IFeature? _lastSelectFeature;
    private IFeature? _lastHoverFeature;
    private Action<IFeature>? _selectAction;
    private Action<IFeature>? _unselectAction;
    private Action<IFeature>? _enterAction;
    private Action<IFeature>? _leaveAction;

    public FeatureManager OnLayer(ILayer? layer)
    {
        _layer = layer;

        return this;
    }

    public FeatureManager WithSelect(Action<IFeature> action)
    {
        _selectAction = action;

        return this;
    }

    public FeatureManager WithUnselect(Action<IFeature> action)
    {
        _unselectAction = action;

        return this;
    }

    public FeatureManager WithEnter(Action<IFeature> action)
    {
        _enterAction = action;

        return this;
    }

    public FeatureManager WithLeave(Action<IFeature> action)
    {
        _leaveAction = action;

        return this;
    }

    public void Select(IFeature? feature)
    {
        if (feature != null)
        {
            if (_lastSelectFeature != null)
            {
                _unselectAction?.Invoke(_lastSelectFeature);

                _lastSelectLayer?.DataHasChanged();
            }

            _selectAction?.Invoke(feature);

            _lastSelectFeature = feature;

            _lastSelectLayer = _layer;

            _layer?.DataHasChanged();
        }
    }

    public void Unselect()
    {
        if (_lastSelectFeature != null)
        {
            _unselectAction?.Invoke(_lastSelectFeature);

            _lastSelectLayer?.DataHasChanged();
        }
    }

    public void Enter(IFeature? feature)
    {
        if (feature != null)
        {
            if (_lastHoverFeature != null)
            {
                _leaveAction?.Invoke(_lastHoverFeature);

                if (_lastHoverLayer != _layer)
                {
                    _lastHoverLayer?.DataHasChanged();
                }
            }

            _enterAction?.Invoke(feature);

            _lastHoverFeature = feature;

            _lastHoverLayer = _layer;

            _layer?.DataHasChanged();
        }
    }

    public void Leave()
    {
        if (_lastHoverFeature != null)
        {
            _leaveAction?.Invoke(_lastHoverFeature);

            _lastHoverLayer?.DataHasChanged();
        }
    }
}
