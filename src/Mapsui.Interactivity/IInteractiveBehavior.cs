using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class DeltaEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();
    }

    public class StartedEventArgs : EventArgs
    {
        public MPoint WorldPosition { get; set; } = new MPoint();

        public double ScreenDistance { get; set; }
    }

    public class CompletedEventArgs : EventArgs
    {
        public Predicate<MPoint>? IsEnd { get; set; }

        public MapInfo? MapInfo { get; set; }
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

        void OnStarted(MPoint worldPosition, double screenDistance);

        void OnDelta(MPoint worldPosition);

        void OnCompleted(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        void OnCancel();

        void OnHover(MapInfo? mapInfo);

        void OnHoverStart(MapInfo? mapInfo);

        void OnHoverStop(MapInfo? mapInfo);
    }
}
