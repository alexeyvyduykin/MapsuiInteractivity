using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;

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

        public IDecoratingSelector CreateDecoratingSelector(LayerCollection layers, Func<GeometryFeature, IDecorator> builder)
        {
            IDecoratingSelector selector = new DecoratingSelector(layers, builder);

            return selector;
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

        public static IStyle CreateInteractiveLayerDecoratorStyle()
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
