using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI.Avalonia;
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
                _selector?.HoveringBegin(feature, layer);
            }
        }

        public void PointeroverLeaveImpl()
        {
            _selector?.HoveringEnd();
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
            _selector?.Unselected();
            _selector = null;

            Interactive = null;
            State = States.Default;
        }

        private void SelectCommand()
        {
            _selector = new BaseSelector();

            _selector.Select.Subscribe(s =>
            {
                Tip = $"Select{Environment.NewLine}{s.SelectedFeature?.ToFeatureInfo()}";

                if (IsWktInfo == true)
                {
                    WktInfo = s.SelectedFeature?.ToWkt();
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
                Tip = $"HoveringBegin{Environment.NewLine}{s.HoveringFeature?.ToFeatureInfo()}";
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
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new TranslateDecorator(gf));

            ((IDecoratingSelector)_selector).DecoratorSelecting.Subscribe(s =>
            {
                Interactive = s;
                State = States.Editing;
                Tip = $"Translate mode";
            });

            _selector.Unselect.Subscribe(s =>
            {
                Interactive = s;
                State = States.Selecting;
                Tip = String.Empty;
            });

            Interactive = _selector;
            State = States.Selecting;
        }

        private void ScaleCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new ScaleDecorator(gf));

            ((IDecoratingSelector)_selector).DecoratorSelecting.Subscribe(s =>
            {
                Interactive = s;
                State = States.Editing;
                Tip = $"Scale mode";
            });

            _selector.Unselect.Subscribe(s =>
            {
                Interactive = s;
                State = States.Selecting;
                Tip = string.Empty;
            });

            Interactive = _selector;
            State = States.Selecting;
        }

        private void RotateCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new RotateDecorator(gf));

            ((IDecoratingSelector)_selector).DecoratorSelecting.Subscribe(s =>
            {
                Interactive = s;
                State = States.Editing;
                Tip = $"Rotate mode";
            });

            _selector.Unselect.Subscribe(s =>
            {
                Interactive = s;
                State = States.Selecting;
                Tip = string.Empty;
            });

            Interactive = _selector;
            State = States.Selecting;
        }

        private void EditCommand()
        {
            _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new EditDecorator(gf));

            ((IDecoratingSelector)_selector).DecoratorSelecting.Subscribe(s =>
            {
                Interactive = s;
                State = States.Editing;
                Tip = $"Edit mode";
            });

            _selector.Unselect.Subscribe(s =>
            {
                Interactive = s;
                State = States.Selecting;
                Tip = string.Empty;
            });

            Interactive = _selector;
            State = States.Selecting;
        }

        private void DrawingPointCommand()
        {
            var designer = new InteractiveFactory().CreatePointDesigner(Map.Layers);

            designer.EndCreating.Subscribe(s =>
            {
                _userLayer.Add(s.Feature.Copy());
            });

            Tip = "Нажмите, чтобы нарисовать точку";

            Interactive = designer;
            State = States.Drawing;
        }

        private void DrawingRectangleCommand()
        {
            var designer = new InteractiveFactory().CreateRectangleDesigner(Map.Layers);

            designer.HoverCreating.Subscribe(s =>
            {
                var area = ((IAreaDesigner)s).Area();

                Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
            });

            designer.EndCreating.Subscribe(s =>
            {
                _userLayer.Add(s.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Rectangle")).First().IsSelected = false;
            });

            Tip = "Нажмите и перетащите, чтобы нарисовать прямоугольник";

            Interactive = designer;
            State = States.Drawing;
        }

        private void DrawingCircleCommand()
        {
            var designer = new InteractiveFactory().CreateCircleDesigner(Map.Layers);

            designer.HoverCreating.Subscribe(s =>
            {
                var area = ((IAreaDesigner)s).Area();

                Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
            });

            designer.EndCreating.Subscribe(s =>
            {
                _userLayer.Add(s.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Circle")).First().IsSelected = false;
            });

            Tip = "Нажмите и перетащите, чтобы нарисовать круг";

            Interactive = designer;
            State = States.Drawing;
        }

        private void DrawingRouteCommand()
        {
            var designer = new InteractiveFactory().CreateRouteDesigner(Map.Layers);

            designer.HoverCreating.Subscribe(s =>
            {
                var distance = ((IRouteDesigner)s).Distance();

                var res = (distance >= 1) ? $"{distance:N2} km" : $"{distance * 1000.0:N2} m";

                Tip = $"Расстояние: {res}";
            });

            designer.EndCreating.Subscribe(s =>
            {
                _userLayer.Add(s.Feature.Copy());

                Tip = string.Empty;

                RadioButtons.Where(s => string.Equals(s.Name, "Route")).First().IsSelected = false;
            });

            Tip = "Кликните, чтобы начать измерение";

            Interactive = designer;
            State = States.Drawing;
        }

        private void DrawingPolygonCommand()
        {
            var designer = new InteractiveFactory().CreatePolygonDesigner(Map.Layers);

            designer.BeginCreating.Subscribe(s =>
            {
                Tip = "Нажмите, чтобы продолжить рисовать фигуру";
            });

            designer.Creating.Subscribe(s =>
            {
                var area = ((IAreaDesigner)s).Area();

                if (area != 0.0)
                {
                    Tip = $"Щелкните по первой точке, чтобы закрыть эту фигуру. Область: {area:N2} km²";
                }
            });

            designer.EndCreating.Subscribe(s =>
            {
                _userLayer.Add(s.Feature.Copy());

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
}
