namespace Mapsui.Interactivity.UI.Input;

public class MouseDownGesture : InputGesture
{
    public MouseDownGesture(MouseButton mouseButton, int clickCount = 1)
    {
        MouseButton = mouseButton;
        ClickCount = clickCount;
    }

    public MouseButton MouseButton { get; private set; }

    public int ClickCount { get; private set; }

    public override bool Equals(InputGesture? other)
    {
        var mg = other as MouseDownGesture;
        return mg != null && mg.MouseButton == MouseButton && mg.ClickCount == ClickCount;
    }
}