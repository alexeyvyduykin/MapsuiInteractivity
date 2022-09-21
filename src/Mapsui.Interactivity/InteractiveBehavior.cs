using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public class InteractiveBehavior : IInteractiveBehavior
    {
        public event StartedEventHandler? Started;
        public event DeltaEventHandler? Delta;
        public event CompletedEventHandler? Completed;
        public event EventHandler? Cancel;
        public event HoverEventHandler? Hover;
        public event HoverEventHandler? HoverStart;
        public event HoverEventHandler? HoverStop;

        public InteractiveBehavior(IInteractive interactive)
        {
            if (interactive is IDesigner)
            {
                Started += (s, e) =>
                {
                    interactive.Starting(e.MapInfo);
                };
            }
            else if (interactive is IDecorator decorator)
            {
                Started += (s, e) =>
                {
                    var vertices = decorator.GetActiveVertices();

                    var worldPosition = e.MapInfo?.WorldPosition;

                    if (worldPosition != null)
                    {
                        var vertexTouched = vertices.OrderBy(v => v.Distance(worldPosition)).FirstOrDefault(v => v.Distance(worldPosition) < e.ScreenDistance);

                        if (vertexTouched != null)
                        {
                            interactive.Starting(e.MapInfo);
                        }
                    }
                };
            }

            Delta += (s, e) =>
            {
                interactive.Moving(e.MapInfo);
            };

            Completed += (s, e) =>
            {
                interactive.Ending(e.MapInfo, e.IsEnd);
            };

            Cancel += (s, e) =>
            {
                if (interactive is IDecorator decorator)
                {
                    decorator.Canceling();
                }
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
                    selector.PointeroverStop();
                }
            };
        }

        public void OnDelta(MapInfo? mapInfo)
        {
            Delta?.Invoke(this, new DeltaEventArgs()
            {
                MapInfo = mapInfo
            });
        }

        public void OnStarted(MapInfo? mapInfo, double screenDistance)
        {
            Started?.Invoke(this, new StartedEventArgs()
            {
                MapInfo = mapInfo,
                ScreenDistance = screenDistance
            });
        }

        public void OnCompleted(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { MapInfo = mapInfo, IsEnd = isEnd });
        }

        public void OnCancel()
        {
            Cancel?.Invoke(this, new EventArgs());
        }

        public void OnHover(MapInfo? mapInfo)
        {
            Hover?.Invoke(this, new HoverEventArgs()
            {
                MapInfo = mapInfo
            });
        }

        public void OnHoverStart(MapInfo? mapInfo)
        {
            HoverStart?.Invoke(this, new HoverEventArgs()
            {
                MapInfo = mapInfo
            });
        }

        public void OnHoverStop(MapInfo? mapInfo)
        {
            HoverStop?.Invoke(this, new HoverEventArgs()
            {
                MapInfo = mapInfo
            });
        }
    }
}
