using Mapsui;
using System;
using System.Linq;

namespace Mapsui.Interactivity
{
    public class InteractiveBehavior : IInteractiveBehavior
    {
        public event StartedEventHandler? Started;

        public event DeltaEventHandler? Delta;

        public event CompletedEventHandler? Completed;

        public event HoverEventHandler? Hover;

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
                throw new Exception();
            }

            Delta += (s, e) =>
            {
                interactive.Moving(e.WorldPosition);
            };

            Completed += (s, e) =>
            {
                interactive.Ending(e.WorldPosition, e.IsEnd);
            };

            Hover += (s, e) =>
            {
                interactive.Hovering(e.WorldPosition);
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

        public void OnCompleted(MPoint worldPosition, Predicate<MPoint> isEnd)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = isEnd });
        }

        public void OnCompleted(MPoint worldPosition)
        {
            Completed?.Invoke(this, new CompletedEventArgs() { WorldPosition = worldPosition, IsEnd = null });
        }

        public void OnHover(MPoint worldPosition)
        {
            Hover?.Invoke(this, new HoverEventArgs() { WorldPosition = worldPosition });
        }
    }
}
