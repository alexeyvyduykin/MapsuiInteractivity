using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using System.Linq;

namespace Mapsui.Interactivity
{
    public class InteractiveFactory
    {
        public IDesigner CreatePolygonDesigner(IMap map)
        {
            return CreateDesigner(map, new PolygonDesigner());
        }

        public IDesigner CreateRouteDesigner(IMap map)
        {
            return CreateDesigner(map, new RouteDesigner());
        }

        public IDesigner CreateCircleDesigner(IMap map)
        {
            return CreateDesigner(map, new CircleDesigner());
        }

        public IDesigner CreatePointDesigner(IMap map)
        {
            return CreateDesigner(map, new PointDesigner());
        }

        public IDesigner CreateRectangleDesigner(IMap map)
        {
            return CreateDesigner(map, new RectangleDesigner());
        }

        private IDesigner CreateDesigner(IMap map, IDesigner designer)
        {
            RemoveInteractiveLayer(map);

            var interactiveLayer = new InteractiveLayer(designer)
            {
                Name = nameof(InteractiveLayer),
                Style = CreateInteractiveLayerDesignerStyle(),
            };

            designer.BeginCreating += (s, e) =>
            {
                map.Layers.Add(interactiveLayer);
            };

            designer.EndCreating += (s, e) =>
            {
                map.Layers.Remove(interactiveLayer);
            };

            return designer;
        }

        public ISelectDecorator CreateSelectDecorator(Map map, ILayer layer)
        {
            var selectDecorator = new SelectDecorator(map, layer);

            BaseFeature? saveFeature = null;

            selectDecorator.Select += (s, e) =>
            {
                if (s is ISelectDecorator decorator)
                {
                    if (saveFeature != null)
                    {
                        saveFeature["Interactive.Select"] = false;
                    }

                    saveFeature = decorator.SelectedFeature!;
                    saveFeature["Interactive.Select"] = true;
                }
            };

            selectDecorator.Unselect += (s, e) =>
            {
                if (s is ISelectDecorator decorator)
                {
                    if (saveFeature != null)
                    {
                        saveFeature["Interactive.Select"] = false;
                        saveFeature = null;
                    }
                }
            };

            return selectDecorator;
        }

        public ISelectScaleDecorator CreateSelectScaleDecorator(Map map, ILayer layer)
        {
            var scaleDecorator = new SelectScaleDecorator(map, layer);

            scaleDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectScaleDecorator decorator)
                {
                    if (decorator.Scale != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Scale)
                        {
                            Name = nameof(InteractiveLayer),
                            Style = CreateInteractiveLayerDecoratorStyle(),
                        };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return scaleDecorator;
        }

        public ISelectTranslateDecorator CreateSelectTranslateDecorator(Map map, ILayer layer)
        {
            var translateDecorator = new SelectTranslateDecorator(map, layer);

            translateDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectTranslateDecorator decorator)
                {
                    if (decorator.Translate != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Translate)
                        {
                            Name = nameof(InteractiveLayer),
                            Style = CreateInteractiveLayerDecoratorStyle(),
                        };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return translateDecorator;
        }

        public ISelectRotateDecorator CreateSelectRotateDecorator(Map map, ILayer layer)
        {
            var rotateDecorator = new SelectRotateDecorator(map, layer);

            rotateDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectRotateDecorator decorator)
                {
                    if (decorator.Rotate != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Rotate)
                        {
                            Name = nameof(InteractiveLayer),
                            Style = CreateInteractiveLayerDecoratorStyle(),
                        };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return rotateDecorator;
        }

        public ISelectEditDecorator CreateSelectEditDecorator(Map map, ILayer layer)
        {
            var editDecorator = new SelectEditDecorator(map, layer);

            editDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectEditDecorator decorator)
                {
                    if (decorator.Edit != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Edit)
                        {
                            Name = nameof(InteractiveLayer),
                            Style = CreateInteractiveLayerDecoratorStyle(),
                        };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return editDecorator;
        }

        public IDecorator CreateScaleDecorator(GeometryFeature feature)
        {
            var decorator = new ScaleDecorator(feature);

            return decorator;
        }

        public IDecorator CreateTranslateDecorator(GeometryFeature feature)
        {
            var decorator = new TranslateDecorator(feature);

            return decorator;
        }

        public IDecorator CreateRotateDecorator(GeometryFeature feature)
        {
            var decorator = new RotateDecorator(feature);

            return decorator;
        }

        public IDecorator CreateEditDecorator(GeometryFeature feature)
        {
            var decorator = new EditDecorator(feature);

            return decorator;
        }

        private void RemoveInteractiveLayer(IMap map)
        {
            var interactiveLayer = map.Layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                map.Layers.Remove(interactiveLayer);
            }
        }

        private static IStyle CreateInteractiveLayerDesignerStyle()
        {
            return new ThemeStyle(f =>
            {
                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                if (gf.Geometry is Point)
                {
                    return new SymbolStyle()
                    {
                        Fill = new Brush(Color.White),
                        Outline = new Pen(Color.Black, 2 / 0.3),
                        Line = null,
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                    };
                }

                var _color = new Color(76, 154, 231);

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Name") == true)
                        {
                            if (gf["Name"]!.Equals("ExtraPolygonHoverLine"))
                            {
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(_color, 4) { PenStyle = PenStyle.Dot },
                                };
                            }
                            else if (gf["Name"]!.Equals("ExtraPolygonArea"))
                            {
                                return new VectorStyle
                                {
                                    Fill = new Brush(Color.Opacity(_color, 0.25f)),
                                    Line = null,
                                    Outline = null,
                                };
                            }
                            else if (gf["Name"]!.Equals("ExtraRouteHoverLine"))
                            {
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(_color, 3) { PenStyle = PenStyle.Dash },
                                };
                            }
                        }
                    }
                }

                if (gf.Geometry is Polygon || gf.Geometry is LineString)
                {
                    return new VectorStyle
                    {
                        Fill = new Brush(Color.Transparent),
                        Line = new Pen(_color, 3),
                        Outline = new Pen(_color, 3)
                    };
                }

                return null;
            });
        }

        private static IStyle CreateInteractiveLayerDecoratorStyle()
        {
            return new ThemeStyle(f =>
            {
                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                if (gf.Geometry is Point)
                {
                    return new SymbolStyle()
                    {
                        Fill = new Brush(Color.White),
                        Outline = new Pen(Color.Black, 2 / 0.3),
                        Line = null,
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                    };
                }

                return null;
            });
        }
    }
}
