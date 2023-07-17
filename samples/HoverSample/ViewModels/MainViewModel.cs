using HoverSample.Models;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.Utilities;
using System;

namespace HoverSample.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly FeatureManager _featureManager;

    public MainViewModel()
    {
        Map = MapBuilder.Create();

        _featureManager = new FeatureManager()
            .WithSelect(f => f[MapBuilder.SelectField] = true)
            .WithUnselect(f => f[MapBuilder.SelectField] = false)
            .WithEnter(f => f[MapBuilder.HoverField] = true)
            .WithLeave(f => f[MapBuilder.HoverField] = false);

        var selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        selector.Select.Subscribe(s =>
        {
            _featureManager.OnLayer(s.Layer).Select(s.Feature);

            //Tip = $"Select{Environment.NewLine}{s.Feature.ToFeatureInfo()}";
        });

        selector.Unselect.Subscribe(s =>
        {
            _featureManager.OnLayer(s.Layer).Unselect();

            //Tip = string.Empty;
        });

        selector.HoverBegin.Subscribe(s =>
        {
            _featureManager.OnLayer(s.Layer).Enter(s.Feature);

            //Tip = $"HoveringBegin{Environment.NewLine}{s.Feature.ToFeatureInfo()}";
        });

        selector.HoverEnd.Subscribe(s =>
        {
            _featureManager.OnLayer(s.Layer).Leave();

            //Tip = string.Empty;
        });

        Interactive = selector;
        State = States.Selecting;
    }

    public Map Map { get; set; }

    public IInteractive? Interactive { get; set; }

    public string State { get; set; } = States.Default;

    public string Tip { get; set; } = string.Empty;
}
