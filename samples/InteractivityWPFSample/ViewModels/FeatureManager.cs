using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using System;
using System.Linq;

namespace InteractivityWPFSample.ViewModels;

public class FeatureManager
{
    private ILayer? _layer;
    private ILayer? _lastSelectLayer;
    private ILayer? _lastHoverLayer;
    private IFeature? _lastSelectFeature;
    private IFeature? _lastHoverFeature;
    private IStyle? _selectStyle;
    private IStyle? _hoverStyle;
    private Action<IFeature>? _selectAction;
    private Action<IFeature>? _unselectAction;
    private Action<IFeature>? _enterAction;
    private Action<IFeature>? _leaveAction;

    public FeatureManager OnLayer(ILayer? layer)
    {
        _layer = layer;

        return this;
    }

    public FeatureManager WithSelectStyle(IStyle style)
    {
        _selectStyle = style;

        return this;
    }

    public FeatureManager WithHoverStyle(IStyle style)
    {
        _hoverStyle = style;

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

                if (_selectStyle is { })
                {
                    _lastSelectFeature.Styles.Remove(_selectStyle);
                }

                _lastSelectLayer?.DataHasChanged();
            }

            _selectAction?.Invoke(feature);

            if (_selectStyle is { })
            {
                feature.Styles.Add(_selectStyle);
            }

            _lastSelectFeature = feature;

            _lastSelectLayer = _layer;

            _layer?.DataHasChanged();
        }
    }

    public void Unselect()
    {
        //if (_lastPointeroverFeature != null)
        //{
        //    _lastPointeroverFeature[InteractiveFields.Hover] = false;

        //    _lastPointeroverLayer?.DataHasChanged();
        //}

        if (_lastSelectFeature != null)
        {
            _unselectAction?.Invoke(_lastSelectFeature);

            if (_selectStyle is { })
            {
                _lastSelectFeature.Styles.Remove(_selectStyle);
            }

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

                if (_hoverStyle is { })
                {
                    _lastHoverFeature.Styles.Remove(_hoverStyle);
                }

                //     _lastHoverFeature.RenderedGeometry.Clear(); // 111

                if (_lastHoverLayer != _layer)
                {
                    _lastHoverLayer?.DataHasChanged();
                }
            }

            _enterAction?.Invoke(feature);

            if (_hoverStyle is { })
            {
                AddHoverStyleBottom(feature);
            }

            _lastHoverFeature = feature;

            _lastHoverLayer = _layer;

            // _lastHoverFeature.RenderedGeometry.Clear(); // 111

            _layer?.DataHasChanged();
        }
    }

    public void Leave()
    {
        if (_lastHoverFeature != null)
        {
            _leaveAction?.Invoke(_lastHoverFeature);

            if (_hoverStyle is { })
            {
                _lastHoverFeature.Styles.Remove(_hoverStyle);
            }

            //   _lastHoverFeature.RenderedGeometry?.Clear(); // 111

            _lastHoverLayer?.DataHasChanged();
        }
    }

    private void AddHoverStyleBottom(IFeature feature)
    {
        if (_selectStyle is { } && _hoverStyle is { })
        {
            var res = feature.Styles.Contains(_selectStyle);

            if (res == true)
            {
                var origin = feature.Styles.ToList();

                feature.Styles.Clear();

                foreach (var item in origin)
                {
                    if (Equals(item, _selectStyle) == true)
                    {
                        feature.Styles.Add(_hoverStyle);
                    }

                    feature.Styles.Add(item);
                }
            }
            else
            {
                feature.Styles.Add(_hoverStyle);
            }
        }
    }
}