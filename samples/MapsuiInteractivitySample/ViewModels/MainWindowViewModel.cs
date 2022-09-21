﻿using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace MapsuiInteractivitySample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly WritableLayer _userLayer;
        private ISelector? _selector;

        public MainWindowViewModel()
        {
            Map = MapBuilder.Create();

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

            RadioButtons = new List<RadioButtonItem>(radioButtonList.Items);

            ActualController = new DefaultController();

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
                _selector?.PointeroverStart(feature, layer);
            }
        }

        public void PointeroverLeaveImpl()
        {
            _selector?.PointeroverStop();
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
            if (_selector is IDecoratingSelector decoratingSelector)
            {
                decoratingSelector.Decorator?.Canceling();
            }

            _selector?.Unselected();
            _selector = null;

            Behavior = null;
            ActualController = new DefaultController();
        }

        private void SelectCommand()
        {
            _selector = new BaseSelector();

            _selector.Select += (s, e) =>
            {
                if (s is IFeature feature)
                {
                    Tip = $"Select{Environment.NewLine}{feature.ToFeatureInfo()}";

                    if (IsWktInfo == true)
                    {
                        WktInfo = feature.ToWkt();
                    }
                }
            };

            _selector.Unselect += (s, e) =>
            {
                Tip = string.Empty;

                if (IsWktInfo == true)
                {
                    WktInfo = string.Empty;
                }
            };

            _selector.HoveringBegin += (s, e) =>
            {
                if (s is IFeature feature)
                {
                    Tip = $"HoveringBegin{Environment.NewLine}{feature.ToFeatureInfo()}";
                }
            };

            _selector.HoveringEnd += (s, e) =>
            {
                Tip = string.Empty;
            };

            Behavior = new InteractiveBehavior(_selector);
            ActualController = new SelectingController();
        }

        private void TranslateCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new TranslateDecorator(gf));

            ((IDecoratingSelector)_selector).SelectedDecorator += (s, e) =>
            {
                if (s is IDecorator decorator)
                {
                    Behavior = new InteractiveBehavior(decorator);
                    ActualController = new EditingController();
                    Tip = $"Translate mode";
                }
            };

            _selector.Unselect += (s, e) =>
            {
                Behavior = new InteractiveBehavior(_selector);
                ActualController = new SelectingController();
                Tip = String.Empty;
            };

            Behavior = new InteractiveBehavior(_selector);
            ActualController = new SelectingController();
        }

        private void ScaleCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new ScaleDecorator(gf));

            ((IDecoratingSelector)_selector).SelectedDecorator += (s, e) =>
            {
                if (s is IDecorator decorator)
                {
                    Behavior = new InteractiveBehavior(decorator);
                    ActualController = new EditingController();
                    Tip = $"Scale mode";
                }
            };

            _selector.Unselect += (s, e) =>
            {
                Behavior = new InteractiveBehavior(_selector);
                ActualController = new SelectingController();
                Tip = string.Empty;
            };

            Behavior = new InteractiveBehavior(_selector);
            ActualController = new SelectingController();
        }

        private void RotateCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new RotateDecorator(gf));

            ((IDecoratingSelector)_selector).SelectedDecorator += (s, e) =>
            {
                if (s is IDecorator decorator)
                {
                    Behavior = new InteractiveBehavior(decorator);
                    ActualController = new EditingController();
                    Tip = $"Rotate mode";
                }
            };

            _selector.Unselect += (s, e) =>
            {
                Behavior = new InteractiveBehavior(_selector);
                ActualController = new SelectingController();
                Tip = string.Empty;
            };

            Behavior = new InteractiveBehavior(_selector);
            ActualController = new SelectingController();
        }

        private void EditCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new EditDecorator(gf));

            ((IDecoratingSelector)_selector).SelectedDecorator += (s, e) =>
            {
                if (s is IDecorator decorator)
                {
                    Behavior = new InteractiveBehavior(decorator);
                    ActualController = new EditingController();
                    Tip = $"Edit mode";
                }
            };

            _selector.Unselect += (s, e) =>
            {
                Behavior = new InteractiveBehavior(_selector);
                ActualController = new SelectingController();
                Tip = string.Empty;
            };

            Behavior = new InteractiveBehavior(_selector);
            ActualController = new SelectingController();
        }

        private void DrawingPointCommand()
        {
            var designer = new InteractiveFactory().CreatePointDesigner(Map.Layers);

            designer.EndCreating += (s, e) =>
            {
                _userLayer.Add(designer.Feature.Copy());
            };

            Tip = "Нажмите, чтобы нарисовать точку";

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingRectangleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateRectangleDesigner(Map.Layers);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayer.Add(designer.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Rectangle")).First().IsSelected = false;
            };

            Tip = "Нажмите и перетащите, чтобы нарисовать прямоугольник";

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingCircleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateCircleDesigner(Map.Layers);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayer.Add(designer.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Circle")).First().IsSelected = false;
            };

            Tip = "Нажмите и перетащите, чтобы нарисовать круг";

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingRouteCommand()
        {
            var designer = (IRouteDesigner)new InteractiveFactory().CreateRouteDesigner(Map.Layers);

            designer.HoverCreating += (s, e) =>
            {
                var distance = designer.Distance();

                var res = (distance >= 1) ? $"{distance:N2} km" : $"{distance * 1000.0:N2} m";

                Tip = $"Расстояние: {res}";
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayer.Add(designer.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Route")).First().IsSelected = false;
            };

            Tip = "Кликните, чтобы начать измерение";

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingPolygonCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreatePolygonDesigner(Map.Layers);

            designer.BeginCreating += (s, e) =>
            {
                Tip = "Нажмите, чтобы продолжить рисовать фигуру";
            };

            designer.Creating += (s, e) =>
            {
                var area = designer.Area();

                if (area != 0.0)
                {
                    Tip = $"Щелкните по первой точке, чтобы закрыть эту фигуру. Область: {area:N2} km²";
                }
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayer.Add(designer.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Polygon")).First().IsSelected = false;
            };

            Tip = "Нажмите и перетащите, чтобы нарисовать полигон";

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        [Reactive]
        public Map Map { get; set; }

        [Reactive]
        public IList<LayerViewModel> Layers { get; set; }

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
        public IController ActualController { get; set; }

        [Reactive]
        public IInteractiveBehavior? Behavior { get; set; }

        [Reactive]
        public string Tip { get; set; } = string.Empty;

        [Reactive]
        public bool IsWktInfo { get; set; }

        [Reactive]
        public string? WktInfo { get; set; }
    }
}
