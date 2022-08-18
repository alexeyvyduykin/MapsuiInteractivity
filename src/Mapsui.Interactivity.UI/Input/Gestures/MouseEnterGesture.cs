namespace Mapsui.Interactivity.UI.Input
{
    public class MouseEnterGesture : InputGesture
    {
        public MouseEnterGesture()
        {

        }

        public override bool Equals(InputGesture? other)
        {
            var mg = other as MouseEnterGesture;
            return mg != null;
        }
    }
}