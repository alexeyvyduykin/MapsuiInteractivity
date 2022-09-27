using Mapsui.Nts;

namespace Mapsui.Interactivity
{
    public partial class InteractiveBuilder
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

            _cache1.Add(typeof(BaseSelector), () => new BaseSelector());

            _cache2.Add(typeof(TranslateDecorator), gf => new TranslateDecorator(gf));
            _cache2.Add(typeof(ScaleDecorator), gf => new ScaleDecorator(gf));
            _cache2.Add(typeof(RotateDecorator), gf => new RotateDecorator(gf));
            _cache2.Add(typeof(EditDecorator), gf => new EditDecorator(gf));
        }
    }
}
