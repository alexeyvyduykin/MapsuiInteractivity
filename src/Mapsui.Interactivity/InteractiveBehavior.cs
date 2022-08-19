using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class InteractiveBehavior : IInteractiveBehavior
    {
        public event StartedEventHandler? Started;
        public event DeltaEventHandler? Delta;
        public event CompletedEventHandler? Completed;
        public event HoverEventHandler? Hover;
        public event HoverEventHandler? HoverStart;
        public event HoverEventHandler? HoverStop;

        public InteractiveBehavior(IInteractive interactive)
        {
            if (interactive is IDesigner)
            {
                Started += (s, e) =>
                {
                    interactive.Starting(e.WorldPosition);
                };
            }
            else if (interactive is IDecorator decorator)
            {
                Started += (s, e) =>
                {
                    var vertices = decorator.GetActiveVertices();

                    var vertexTouched = vertices.OrderBy(v => v.Distance(e.WorldPosition)).FirstOrDefault(v => v.Distance(e.WorldPosition) < e.ScreenDistance);

                    if (vertexTouched != null)
                    {
                        interactive.Starting(e.WorldPosition);
                    }
                };
            }
            else
            {
                //throw new Exception();
            }

            Delta += (s, e) =>
            {
                interactive.Moving(e.WorldPosition);
            };

            Completed += (s, e) =>
            {                    
                interactive.Ending(e.MapInfo, e.IsEnd);                
            };

            Hover += (s, e) =>
            {
                interactive.Hovering(e.MapInfo);
            };

            HoverStart += (s, e) =>
            {
                if (interactive is ISelector selector)
                {
                    selector.PointeroverStart(e.MapInfo);
                }
            };

            HoverStop += (s, e) =>
            {
                if (interactive is ISelector selector)
                {
                    selector.PointeroverStop(e.MapInfo);
                }
            };
        }

        public void OnDelta(MPoint worldPosition)
        {
            Delta?.Invoke(this, new DeltaEventArgs() { WorldPosition = worldPosition });
        }

        public void OnStarted(MPoint worldPosition, double screenDistance)
        {
            Started?.Invoke(this, new StartedEventArgs() { WorldPosition = worldPosition, ScreenDistance = screenDistance });
        }

        public void OnCompleted(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { MapInfo = mapInfo, IsEnd = isEnd });
        }

        public void OnHover(MapInfo? mapInfo)
        {
            Hover?.Invoke(this, new HoverEventArgs() { MapInfo = mapInfo });
        }

        public void OnHoverStart(MapInfo? mapInfo)
        {
            HoverStart?.Invoke(this, new HoverEventArgs() { MapInfo = mapInfo });
        }

        public void OnHoverStop(MapInfo? mapInfo)
        {
            HoverStop?.Invoke(this, new HoverEventArgs() { MapInfo = mapInfo });
        }
    }
}
