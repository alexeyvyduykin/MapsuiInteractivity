using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class InteractiveFactory
    {
        public IDesigner CreatePolygonDesigner(LayerCollection layers)
        {
            return CreateDesigner(layers, new PolygonDesigner());
        }

        public IDesigner CreateRouteDesigner(LayerCollection layers)
        {
            return CreateDesigner(layers, new RouteDesigner());
        }

        public IDesigner CreateCircleDesigner(LayerCollection layers)
        {
            return CreateDesigner(layers, new CircleDesigner());
        }

        public IDesigner CreatePointDesigner(LayerCollection layers)
        {
            return CreateDesigner(layers, new PointDesigner());
        }

        public IDesigner CreateRectangleDesigner(LayerCollection layers)
        {
            return CreateDesigner(layers, new RectangleDesigner());
        }

        private IDesigner CreateDesigner(LayerCollection layers, IDesigner designer)
        {
            RemoveInteractiveLayer(layers);

            var interactiveLayer = new InteractiveLayer(designer)
            {
                Name = nameof(InteractiveLayer),
                Style = CreateInteractiveLayerDesignerStyle(),
            };

            designer.BeginCreating.Subscribe(_ => layers.Add(interactiveLayer));

            designer.EndCreating.Subscribe(_ => layers.Remove(interactiveLayer));

            return designer;
        }

        public IDecoratingSelector CreateDecoratingSelector(LayerCollection layers, Func<GeometryFeature, IDecorator> builder)
        {
            IDecoratingSelector selector = new DecoratingSelector(layers, builder);

            return selector;
        }

        private void RemoveInteractiveLayer(LayerCollection layers)
        {
            var interactiveLayer = layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                layers.Remove(interactiveLayer);
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
