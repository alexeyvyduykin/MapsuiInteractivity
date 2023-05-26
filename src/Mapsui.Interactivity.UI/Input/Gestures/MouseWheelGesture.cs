namespace Mapsui.Interactivity.UI.Input;

public class MouseWheelGesture : InputGesture
{
    public MouseWheelGesture()
    {

    }

    public override bool Equals(InputGesture? other)
    {
        var mwg = other as MouseWheelGesture;
        return mwg != null;
    }
}