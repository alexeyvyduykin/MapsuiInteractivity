using Mapsui.Interactivity.Interfaces;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;

namespace Mapsui.Interactivity
{
    public class InteractiveBuilder
    {
        private static readonly IDictionary<Type, Func<IInteractive>> _cache1 = new Dictionary<Type, Func<IInteractive>>();
        private static readonly IDictionary<Type, Func<GeometryFeature, IDecorator>> _cache2 = new Dictionary<Type, Func<GeometryFeature, IDecorator>>();

        static InteractiveBuilder()
        {
            _cache1.Add(typeof(PointDesigner), () => new PointDesigner());
            _cache1.Add(typeof(RectangleDesigner), () => new RectangleDesigner());
            _cache1.Add(typeof(CircleDesigner), () => new CircleDesigner());
            _cache1.Add(typeof(PolygonDesigner), () => new PolygonDesigner());
            _cache1.Add(typeof(RouteDesigner), () => new RouteDesigner());

            _cache1.Add(typeof(Selector), () => new Selector());

            _cache2.Add(typeof(TranslateDecorator), gf => new TranslateDecorator(gf));
            _cache2.Add(typeof(ScaleDecorator), gf => new ScaleDecorator(gf));
            _cache2.Add(typeof(RotateDecorator), gf => new RotateDecorator(gf));
            _cache2.Add(typeof(EditDecorator), gf => new EditDecorator(gf));
        }

        public IDecoratorBuilder SelectDecorator<T>() where T : IDecorator
        {
            var type = typeof(T);

            if (_cache2.ContainsKey(type) == true)
            {
                return new DecoratorBuilder(_cache2[type]);
            }

            throw new Exception($"Decorator type {type} not register in {nameof(InteractiveBuilder)}'s cache.");
        }

        public IDesignerBuilder SelectDesigner<T>() where T : IDesigner
        {
            var type = typeof(T);

            if (_cache1.ContainsKey(type) == true)
            {
                return new DesignerBuilder(_cache1[type]);
            }

            throw new Exception($"Designer type {type} not register in {nameof(InteractiveBuilder)}'s cache.");
        }

        public ISelectorBuilder SelectSelector<T>() where T : ISelector
        {
            var type = typeof(T);

            if (_cache1.ContainsKey(type) == true)
            {
                return new SelectorBuilder(_cache1[type]);
            }

            throw new Exception($"Selector type {type} not register in {nameof(InteractiveBuilder)}'s cache.");
        }

        internal static IStyle CreateInteractiveLayerDecoratorStyle()
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

        internal static IStyle CreateInteractiveLayerDesignerStyle()
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
    }
}
