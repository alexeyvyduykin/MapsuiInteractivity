namespace Mapsui.Interactivity.UI;

public static class InteractiveControllerFactory
{
    private static readonly IDictionary<string, Func<IController>> _cache = new Dictionary<string, Func<IController>>();

    static InteractiveControllerFactory()
    {
        _cache.Add(States.Default, () => new DefaultController());
        _cache.Add(States.Selecting, () => new SelectingController());
        _cache.Add(States.Drawing, () => new DrawingController());
        _cache.Add(States.Editing, () => new EditingController());
    }

    public static void Clear()
    {
        _cache.Clear();
    }

    public static void Register(string key, Func<IController> builder)
    {
        if (_cache.ContainsKey(key) == false)
        {
            _cache.Add(key, builder);
        }
    }

    public static IController GetController(string key)
    {
        if (_cache.ContainsKey(key) == false)
        {
            return _cache[States.Default].Invoke();
        }

        return _cache[key].Invoke();
    }
}
