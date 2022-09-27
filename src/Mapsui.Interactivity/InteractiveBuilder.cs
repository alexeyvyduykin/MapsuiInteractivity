using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using System.Reactive.Linq;

namespace Mapsui.Interactivity
{
    public partial class InteractiveBuilder
    {
        public T Build<T>() where T : IInteractive
        {
            var type = typeof(T);

            if (_cache1.ContainsKey(type) == true)
            {
                if (type.IsAssignableTo(typeof(IDesigner)) == true)
                {
                    return (T)CreateDesigner(type);
                }

                if (type.IsAssignableTo(typeof(ISelector)) == true)
                {
                    return (T)CreateSelectorWithDecorator(type);
                }

                return (T)_cache1[type].Invoke();
            }
            else if (_cache2.ContainsKey(type) == true)
            {
                if (GeometryFeature != null)
                {
                    return (T)_cache2[type].Invoke(GeometryFeature);
                }
            }

            throw new Exception($"Type {type} not register in {nameof(InteractiveBuilder)}'s cache.");
        }

        private IDesigner CreateDesigner(Type type)
        {
            var designer = (IDesigner)_cache1[type].Invoke();

            if (Layers != null)
            {
                designer = AttachDesigner(designer, Layers);
            }

            return designer;
        }

        private ISelector CreateSelectorWithDecorator(Type type)
        {
            ISelector? selector = null;

            if (Layers != null && WrappedDecoratorType != null && _cache2.ContainsKey(WrappedDecoratorType) == true)
            {
                selector = new DecoratorSelector(_cache2[WrappedDecoratorType]);

                ((IDecoratorSelector)selector).DecoratorSelecting.Subscribe(s => AddInteractiveLayer(Layers, s));

                selector.Unselect.Subscribe(_ => RemoveInteractiveLayer(Layers));
            }

            return selector ?? (ISelector)_cache1[type].Invoke();
        }

        private static IDesigner AttachDesigner(IDesigner designer, LayerCollection layers)
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

        private static void AddInteractiveLayer(LayerCollection layers, IDecorator decorator)
        {
            var interactiveLayer = new InteractiveLayer(decorator)
            {
                Name = nameof(InteractiveLayer),
                Style = CreateInteractiveLayerDecoratorStyle(),
            };

            layers.Add(interactiveLayer);
        }

        private static void RemoveInteractiveLayer(LayerCollection layers)
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
