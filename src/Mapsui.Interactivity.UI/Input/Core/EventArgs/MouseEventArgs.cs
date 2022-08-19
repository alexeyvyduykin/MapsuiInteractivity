using Mapsui.UI;

namespace Mapsui.Interactivity.UI.Input.Core
{
    public class MouseEventArgs : InputEventArgs
    {
        public MPoint Position { get; set; } = new MPoint();

        public IView? View { get; set; }

        public MapInfo? MapInfo { get; set; }
    }
}