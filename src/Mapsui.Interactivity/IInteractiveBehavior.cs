using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class DeltaEventArgs : EventArgs
    {
        public MapInfo? MapInfo { get; set; }
    }

    public class StartedEventArgs : EventArgs
    {
        public MapInfo? MapInfo { get; set; }

        public double ScreenDistance { get; set; }
    }

    public class CompletedEventArgs : EventArgs
    {
        public MapInfo? MapInfo { get; set; }

        public Predicate<MPoint>? IsEnd { get; set; }
    }

    public class HoverEventArgs : EventArgs
    {
        public MapInfo? MapInfo { get; set; }
    }

    public delegate void StartedEventHandler(object sender, StartedEventArgs e);
    public delegate void DeltaEventHandler(object sender, DeltaEventArgs e);
    public delegate void CompletedEventHandler(object sender, CompletedEventArgs e);
    public delegate void HoverEventHandler(object sender, HoverEventArgs e);

    public interface IInteractiveBehavior
    {
        event StartedEventHandler? Started;
        event DeltaEventHandler? Delta;
        event CompletedEventHandler? Completed;
        event EventHandler? Cancel;
        event HoverEventHandler? Hover;
        event HoverEventHandler? HoverStart;
        event HoverEventHandler? HoverStop;

        void OnStarted(MapInfo? mapInfo, double screenDistance);

        void OnDelta(MapInfo? mapInfo);

        void OnCompleted(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        void OnCancel();

        void OnHover(MapInfo? mapInfo);

        void OnHoverStart(MapInfo? mapInfo);

        void OnHoverStop(MapInfo? mapInfo);
    }
}
