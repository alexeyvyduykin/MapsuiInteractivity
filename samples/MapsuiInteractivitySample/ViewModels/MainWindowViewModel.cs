﻿using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace MapsuiInteractivitySample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly WritableLayer _userLayer;
        private IDecoratingSelector? _selector;

        public MainWindowViewModel()
        {
            Map = MapBuilder.Create();

            _userLayer = (WritableLayer)Map.Layers[0];

            this.WhenAnyValue(s => s.IsSelect).Subscribe(s => ResetExclude(s, nameof(IsSelect)));
            this.WhenAnyValue(s => s.IsSelect).Subscribe(s => SelectCommand(s));

            this.WhenAnyValue(s => s.IsTranslate).Subscribe(s => ResetExclude(s, nameof(IsTranslate)));
            this.WhenAnyValue(s => s.IsTranslate).Subscribe(s => TranslateCommand(s));

            this.WhenAnyValue(s => s.IsRotate).Subscribe(s => ResetExclude(s, nameof(IsRotate)));
            this.WhenAnyValue(s => s.IsRotate).Subscribe(s => RotateCommand(s));

            this.WhenAnyValue(s => s.IsScale).Subscribe(s => ResetExclude(s, nameof(IsScale)));
            this.WhenAnyValue(s => s.IsScale).Subscribe(s => ScaleCommand(s));

            this.WhenAnyValue(s => s.IsEdit).Subscribe(s => ResetExclude(s, nameof(IsEdit)));
            this.WhenAnyValue(s => s.IsEdit).Subscribe(s => EditCommand(s));

            this.WhenAnyValue(s => s.IsPoint).Subscribe(s => ResetExclude(s, nameof(IsPoint)));
            this.WhenAnyValue(s => s.IsPoint).Subscribe(s => DrawingPointCommand(s));

            this.WhenAnyValue(s => s.IsRectangle).Subscribe(s => ResetExclude(s, nameof(IsRectangle)));
            this.WhenAnyValue(s => s.IsRectangle).Subscribe(s => DrawingRectangleCommand(s));

            this.WhenAnyValue(s => s.IsCircle).Subscribe(s => ResetExclude(s, nameof(IsCircle)));
            this.WhenAnyValue(s => s.IsCircle).Subscribe(s => DrawingCircleCommand(s));

            this.WhenAnyValue(s => s.IsPolygon).Subscribe(s => ResetExclude(s, nameof(IsPolygon)));
            this.WhenAnyValue(s => s.IsPolygon).Subscribe(s => DrawingPolygonCommand(s));

            this.WhenAnyValue(s => s.IsRoute).Subscribe(s => ResetExclude(s, nameof(IsRoute)));
            this.WhenAnyValue(s => s.IsRoute).Subscribe(s => DrawingRouteCommand(s));

            ActualController = new DefaultController();
        }

        private void ResetExclude(bool propertyValue, string propertyName)
        {
            ActualController = new DefaultController();

            Tip = string.Empty;

            if (propertyValue == true)
            {
                if (nameof(MainWindowViewModel.IsSelect) != propertyName)
                {
                    IsSelect = false;
                }

                if (nameof(MainWindowViewModel.IsScale) != propertyName)
                {
                    IsScale = false;
                }

                if (nameof(MainWindowViewModel.IsRotate) != propertyName)
                {
                    IsRotate = false;
                }

                if (nameof(MainWindowViewModel.IsTranslate) != propertyName)
                {
                    IsTranslate = false;
                }

                if (nameof(MainWindowViewModel.IsEdit) != propertyName)
                {
                    IsEdit = false;
                }

                if (nameof(MainWindowViewModel.IsPoint) != propertyName)
                {
                    IsPoint = false;
                }

                if (nameof(MainWindowViewModel.IsRectangle) != propertyName)
                {
                    IsRectangle = false;
                }

                if (nameof(MainWindowViewModel.IsCircle) != propertyName)
                {
                    IsCircle = false;
                }

                if (nameof(MainWindowViewModel.IsPolygon) != propertyName)
                {
                    IsPolygon = false;
                }

                if (nameof(MainWindowViewModel.IsRoute) != propertyName)
                {
                    IsRoute = false;
                }
            }
        }

        private void SelectCommand(bool value)
        {
            if (value == true)
            {
                var selector = new BaseSelector();

                selector.Select += (s, e) =>
                {
                    if (s is IFeature feature)
                    {
                        Tip = feature.ToFeatureInfo();
                    }
                };

                selector.Unselect += (s, e) =>
                {
                    if (s is IFeature feature)
                    {
                        Tip = string.Empty;
                    }
                };

                Behavior = new InteractiveBehavior(selector);
                ActualController = new CustomController();
            }
            else
            {
                Behavior = null;
                ActualController = new DefaultController();
            }
        }

        private void TranslateCommand(bool value)
        {
            if (value == true)
            {
                _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new TranslateDecorator(gf));

                _selector.SelectedDecorator += (s, e) =>
                {
                    if (s is IDecorator decorator)
                    {
                        Behavior = new InteractiveBehavior(decorator);
                        ActualController = new EditController();
                        Tip = $"Translate mode";
                    }
                };

                _selector.Unselect += (s, e) =>
                {
                    Behavior = new InteractiveBehavior(_selector);
                    ActualController = new CustomController();
                    Tip = String.Empty;
                };

                Behavior = new InteractiveBehavior(_selector);
                ActualController = new CustomController();
            }
            else
            {
                _selector?.Decorator?.Canceling();
                _selector?.Unselected();
                Behavior = null;
                ActualController = new DefaultController();
            }
        }

        private void ScaleCommand(bool value)
        {
            if (value == true)
            {
                _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new ScaleDecorator(gf));

                _selector.SelectedDecorator += (s, e) =>
                {
                    if (s is IDecorator decorator)
                    {
                        Behavior = new InteractiveBehavior(decorator);
                        ActualController = new EditController();
                        Tip = $"Scale mode";
                    }
                };

                _selector.Unselect += (s, e) =>
                {
                    Behavior = new InteractiveBehavior(_selector);
                    ActualController = new CustomController();
                    Tip = string.Empty;
                };

                Behavior = new InteractiveBehavior(_selector);
                ActualController = new CustomController();
            }
            else
            {
                _selector?.Decorator?.Canceling();
                _selector?.Unselected();
                Behavior = null;
                ActualController = new DefaultController();
            }
        }

        private void RotateCommand(bool value)
        {
            if (value == true)
            {
                _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new RotateDecorator(gf));

                _selector.SelectedDecorator += (s, e) =>
                {
                    if (s is IDecorator decorator)
                    {
                        Behavior = new InteractiveBehavior(decorator);
                        ActualController = new EditController();
                        Tip = $"Rotate mode";
                    }
                };

                _selector.Unselect += (s, e) =>
                {
                    Behavior = new InteractiveBehavior(_selector);
                    ActualController = new CustomController();
                    Tip = string.Empty;
                };

                Behavior = new InteractiveBehavior(_selector);
                ActualController = new CustomController();
            }
            else
            {
                _selector?.Decorator?.Canceling();
                _selector?.Unselected();
                Behavior = null;
                ActualController = new DefaultController();
            }
        }

        private void EditCommand(bool value)
        {
            if (value == true)
            {
                _selector = new InteractiveFactory().CreateDecoratingSelector(Map.Layers, gf => new EditDecorator(gf));

                _selector.SelectedDecorator += (s, e) =>
                {
                    if (s is IDecorator decorator)
                    {
                        Behavior = new InteractiveBehavior(decorator);
                        ActualController = new EditController();
                        Tip = $"Edit mode";
                    }
                };

                _selector.Unselect += (s, e) =>
                {
                    Behavior = new InteractiveBehavior(_selector);
                    ActualController = new CustomController();
                    Tip = string.Empty;
                };

                Behavior = new InteractiveBehavior(_selector);
                ActualController = new CustomController();
            }
            else
            {
                _selector?.Decorator?.Canceling();
                _selector?.Unselected();
                Behavior = null;
                ActualController = new DefaultController();
            }
        }

        private void DrawingPointCommand(bool value)
        {
            if (value == true)
            {
                var designer = new InteractiveFactory().CreatePointDesigner(Map);

                designer.EndCreating += (s, e) =>
                {
                    _userLayer.Add(designer.Feature.Copy());
                };

                Tip = "Нажмите, чтобы нарисовать точку";

                Behavior = new InteractiveBehavior(designer);

                ActualController = new DrawingController();
            }
        }

        private void DrawingRectangleCommand(bool value)
        {
            if (value == true)
            {
                var designer = (IAreaDesigner)new InteractiveFactory().CreateRectangleDesigner(Map);

                designer.HoverCreating += (s, e) =>
                {
                    var area = designer.Area();

                    Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
                };

                designer.EndCreating += (s, e) =>
                {
                    _userLayer.Add(designer.Feature.Copy());

                    Tip = string.Empty;

                    IsRectangle = false;
                };

                Tip = "Нажмите и перетащите, чтобы нарисовать прямоугольник";

                Behavior = new InteractiveBehavior(designer);

                ActualController = new DrawingController();
            }
        }

        private void DrawingCircleCommand(bool value)
        {
            if (value == true)
            {
                var designer = (IAreaDesigner)new InteractiveFactory().CreateCircleDesigner(Map);

                designer.HoverCreating += (s, e) =>
                {
                    var area = designer.Area();

                    Tip = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²";
                };

                designer.EndCreating += (s, e) =>
                {
                    _userLayer.Add(designer.Feature.Copy());

                    Tip = string.Empty;

                    IsCircle = false;
                };

                Tip = "Нажмите и перетащите, чтобы нарисовать круг";

                Behavior = new InteractiveBehavior(designer);

                ActualController = new DrawingController();
            }
        }

        private void DrawingRouteCommand(bool value)
        {
            if (value == true)
            {
                var designer = (IRouteDesigner)new InteractiveFactory().CreateRouteDesigner(Map);

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

                    IsRoute = false;
                };

                Tip = "Кликните, чтобы начать измерение";

                Behavior = new InteractiveBehavior(designer);

                ActualController = new DrawingController();
            }
        }

        private void DrawingPolygonCommand(bool value)
        {
            if (value == true)
            {
                var designer = (IAreaDesigner)new InteractiveFactory().CreatePolygonDesigner(Map);

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

                    IsPolygon = false;
                };

                Tip = "Нажмите и перетащите, чтобы нарисовать полигон";

                Behavior = new InteractiveBehavior(designer);

                ActualController = new DrawingController();
            }
        }

        [Reactive]
        public Map Map { get; set; }

        [Reactive]
        public bool IsSelect { get; set; } = false;

        [Reactive]
        public bool IsTranslate { get; set; } = false;

        [Reactive]
        public bool IsRotate { get; set; } = false;

        [Reactive]
        public bool IsScale { get; set; } = false;

        [Reactive]
        public bool IsEdit { get; set; } = false;

        [Reactive]
        public bool IsPoint { get; set; } = false;

        [Reactive]
        public bool IsRectangle { get; set; } = false;

        [Reactive]
        public bool IsCircle { get; set; } = false;

        [Reactive]
        public bool IsPolygon { get; set; } = false;

        [Reactive]
        public bool IsRoute { get; set; } = false;

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public IInteractiveBehavior? Behavior { get; set; }

        [Reactive]
        public string Tip { get; set; } = string.Empty;
    }
}
