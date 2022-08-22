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

            Cancel += (s, e) =>
            {
                if (interactive is IDecorator decorator)
                {
                    //     decorator.Dispose(null);
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
                    selector.PointeroverStop(e.MapInfo);
                }
            };
        }

        private void InteractiveBehavior_Dispose(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
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

        public void OnCancel()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
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

    public class NewInteractiveBehavior : IInteractiveBehavior
    {
        public event StartedEventHandler? Started;
        public event DeltaEventHandler? Delta;
        public event CompletedEventHandler? Completed;
        public event HoverEventHandler? Hover;
        public event HoverEventHandler? HoverStart;
        public event HoverEventHandler? HoverStop;
        public event EventHandler? Cancel;

        public NewInteractiveBehavior(ISelector selector, IInteractive interactive)
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
                if (interactive is ISelector selector1)
                {
                    selector1.PointeroverStart(e.MapInfo);
                }
            };

            HoverStop += (s, e) =>
            {
                if (interactive is ISelector selector2)
                {
                    selector2.PointeroverStop(e.MapInfo);
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

        public void OnCancel()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }
    }

    public class NewNewInteractiveBehavior : IInteractiveBehavior
    {
        public event StartedEventHandler? Started;
        public event DeltaEventHandler? Delta;
        public event CompletedEventHandler? Completed;
        public event EventHandler? Cancel;
        public event HoverEventHandler? Hover;
        public event HoverEventHandler? HoverStart;
        public event HoverEventHandler? HoverStop;

        public NewNewInteractiveBehavior(IInteractive interactive)
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
                    selector.PointeroverStop(e.MapInfo);
                }
            };
        }

        private void InteractiveBehavior_Dispose(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
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

        public void OnCancel()
        {
            Cancel?.Invoke(this, new EventArgs());
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
