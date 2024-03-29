﻿using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace MapsuiInteractivitySample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly WritableLayer _userLayer;
    private ISelector? _selector;
    private readonly FeatureManager _featureManager;

    public MainWindowViewModel()
    {
        Map = MapBuilder.Create();

        Map.Layers.Changed += Layers_Changed;

        _featureManager = new FeatureManager()
            .WithSelect(f => f[InteractiveFields.Select] = true)
            .WithUnselect(f => f[InteractiveFields.Select] = false)
            .WithEnter(f => f[InteractiveFields.Hover] = true)
            .WithLeave(f => f[InteractiveFields.Hover] = false);

        _userLayer = (WritableLayer)Map.Layers[0];

        var radioButtonList = new RadioButtonList();

        radioButtonList.Register(new RadioButtonItem("Select"), SelectCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Translate"), TranslateCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Scale"), ScaleCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Rotate"), RotateCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Edit"), EditCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Point"), DrawingPointCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Rectangle"), DrawingRectangleCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Circle"), DrawingCircleCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Polygon"), DrawingPolygonCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Route"), DrawingRouteCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Select|Filter"), SelectWithFilterCommand, Reset);
        radioButtonList.Register(new RadioButtonItem("Translate|Filter"), TranslateWithFilterCommand, Reset);

        RadioButtons = new List<RadioButtonItem>(radioButtonList.Items);

        this.WhenAnyValue(s => s.SelectedLayer)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Subscribe(s =>
            {
                Features = s!.GetFeatures().Select(s => new FeatureViewModel(s)).ToList();
                SelectedFeature = Features.FirstOrDefault();
            });

        this.WhenAnyValue(s => s.SelectedFeature)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Select(s => s!.Name)
            .Subscribe(name => SelectFeatureImpl(name));

        PointerEnterFeature = ReactiveCommand.Create<string>(PointeroverEnterImpl);

        PointerLeaveFeature = ReactiveCommand.Create(PointeroverLeaveImpl);

        Layers = Map.Layers.Select(s => new LayerViewModel(s)).ToList();

        SelectedLayer = Layers.FirstOrDefault();
    }

    private void Layers_Changed(object sender, LayerCollectionChangedEventArgs args)
    {
        if (sender is LayerCollection layers)
        {
            LayerNames = layers.Select(s => s.Name).ToList();
        }
    }

    private void SelectFeatureImpl(string name)
    {
        var (feature, layer) = Find(name, SelectedLayer?.Name);

        if (feature != null && layer != null)
        {
            _selector?.Selected(feature, layer);
        }
    }

    public void PointeroverEnterImpl(string name)
    {
        var (feature, layer) = Find(name, SelectedLayer?.Name);

        if (feature != null && layer != null)
        {
            _selector?.HoveringBegin(feature, layer);

            _featureManager
                .OnLayer(layer)
                .Enter(feature);
        }
    }

    public void PointeroverLeaveImpl()
    {
        var layerName = SelectedLayer?.Name;
        var layer = Map.Layers.Where(s => string.Equals(s.Name, layerName)).FirstOrDefault();

        _selector?.HoveringEnd();

        _featureManager
            .OnLayer(layer)
            .Leave();
    }

    private (IFeature? feature, ILayer? layer) Find(string? featureName, string? layerName)
    {
        var layer = Map.Layers.Where(s => string.Equals(s.Name, layerName)).FirstOrDefault();

        IFeature? feature = null;

        if (layer is WritableLayer wl)
        {
            feature = wl.GetFeatures().Where(s => string.Equals(s["Name"], featureName)).FirstOrDefault();
        }

        return (feature, layer);
    }

    private void Reset()
    {
        Interactive?.Cancel();
        Interactive = null;

        _selector = null;

        State = States.Default;

        Tip = string.Empty;
    }

    private void SelectCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        _selector.Select.Subscribe(s =>
        {
            SelectFeature(s.Feature, s.Layer);

            Tip = $"Select{Environment.NewLine}{s.Feature.ToFeatureInfo()}";

            if (IsWktInfo == true)
            {
                WktInfo = s.Feature.ToWkt();
            }
        });

        _selector.Unselect.Subscribe(s =>
        {
            UnselectFeature(s.Layer);

            Tip = string.Empty;

            if (IsWktInfo == true)
            {
                WktInfo = string.Empty;
            }
        });

        _selector.HoverBegin.Subscribe(s =>
        {
            EnterFeature(s.Feature, s.Layer);

            Tip = $"HoveringBegin{Environment.NewLine}{s.Feature.ToFeatureInfo()}";
        });

        _selector.HoverEnd.Subscribe(s =>
        {
            LeaveFeature(s.Layer);

            Tip = string.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void EnterFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Enter(feature);
    }

    private void LeaveFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Leave();
    }

    private void SelectFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Select(feature);
    }

    private void UnselectFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Unselect();
    }

    private void SelectWithFilterCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .AvailableFor(_userLayer)
            .Build();

        _selector.Select.Subscribe(s =>
        {
            Tip = $"Select{Environment.NewLine}{s.Feature.ToFeatureInfo()}";

            if (IsWktInfo == true)
            {
                WktInfo = s.Feature.ToWkt();
            }
        });

        _selector.Unselect.Subscribe(s =>
        {
            Tip = string.Empty;

            if (IsWktInfo == true)
            {
                WktInfo = string.Empty;
            }
        });

        _selector.HoverBegin.Subscribe(s =>
        {
            Tip = $"HoveringBegin{Environment.NewLine}{s.Feature.ToFeatureInfo()}";
        });

        _selector.HoverEnd.Subscribe(s =>
        {
            Tip = string.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void TranslateCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectDecorator<TranslateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
            Tip = $"Translate mode";
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            Tip = String.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void TranslateWithFilterCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectDecorator<TranslateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(_userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
            Tip = $"Translate mode";
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            Tip = String.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void ScaleCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectDecorator<ScaleDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
            Tip = $"Scale mode";
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            Tip = string.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void RotateCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectDecorator<RotateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
            Tip = $"Rotate mode";
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            Tip = string.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void EditCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectDecorator<EditDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
            Tip = $"Edit mode";
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            Tip = string.Empty;
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void DrawingPointCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PointDesigner>()
            .AttachTo(Map)
            .Build();

        designer.EndCreating.Subscribe(s =>
        {
            _userLayer.Add(s);
            _userLayer.DataHasChanged();
        });

        Tip = "Нажмите, чтобы нарисовать точку";

        Interactive = designer;
        State = States.Drawing;
    }

    private void DrawingRectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s =>
        {
            Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {s.Area():N2} km²";
        });

        designer.EndCreating.Subscribe(s =>
        {
            _userLayer.Add(s);

            Tip = string.Empty;

            RadioButtons.Where(s => string.Equals(s.Name, "Rectangle")).First().IsSelected = false;
        });

        Tip = "Нажмите и перетащите, чтобы нарисовать прямоугольник";

        Interactive = designer;
        State = States.Drawing;
    }

    private void DrawingCircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s =>
        {
            Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {s.Area():N2} km²";
        });

        designer.EndCreating.Subscribe(s =>
        {
            _userLayer.Add(s);

            Tip = string.Empty;

            RadioButtons.Where(s => string.Equals(s.Name, "Circle")).First().IsSelected = false;
        });

        Tip = "Нажмите и перетащите, чтобы нарисовать круг";

        Interactive = designer;
        State = States.Drawing;
    }

    private void DrawingRouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s =>
        {
            var distance = s.Distance();

            var res = (distance >= 1) ? $"{distance:N2} km" : $"{distance * 1000.0:N2} m";

            Tip = $"Расстояние: {res}";
        });

        designer.EndCreating.Subscribe(s =>
        {
            _userLayer.Add(s);

            Tip = string.Empty;

            RadioButtons.Where(s => string.Equals(s.Name, "Route")).First().IsSelected = false;
        });

        Tip = "Кликните, чтобы начать измерение";

        Interactive = designer;
        State = States.Drawing;
    }

    private void DrawingPolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating.Subscribe(s =>
        {
            Tip = "Нажмите, чтобы продолжить рисовать фигуру";
        });

        designer.Creating.Subscribe(s =>
        {
            var area = s.Area();

            if (area != 0.0)
            {
                Tip = $"Щелкните по первой точке, чтобы закрыть эту фигуру. Область: {area:N2} km²";
            }
        });

        designer.EndCreating.Subscribe(s =>
        {
            _userLayer.Add(s);

            Tip = string.Empty;

            RadioButtons.Where(s => string.Equals(s.Name, "Polygon")).First().IsSelected = false;
        });

        Tip = "Нажмите и перетащите, чтобы нарисовать полигон";

        Interactive = designer;
        State = States.Drawing;
    }

    [Reactive]
    public Map Map { get; set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    [Reactive]
    public IList<LayerViewModel> Layers { get; set; }

    [Reactive]
    public IList<string> LayerNames { get; set; } = new List<string>();

    [Reactive]
    public LayerViewModel? SelectedLayer { get; set; }

    [Reactive]
    public IList<FeatureViewModel> Features { get; set; } = new List<FeatureViewModel>();

    [Reactive]
    public FeatureViewModel? SelectedFeature { get; set; }

    public ReactiveCommand<string, Unit> PointerEnterFeature { get; }

    public ReactiveCommand<Unit, Unit> PointerLeaveFeature { get; }

    [Reactive]
    public List<RadioButtonItem> RadioButtons { get; set; }

    [Reactive]
    public string Tip { get; set; } = string.Empty;

    [Reactive]
    public bool IsWktInfo { get; set; }

    [Reactive]
    public string? WktInfo { get; set; }
}
