namespace ThermalDemo.Draw.Interfaces.Model
{
    public interface IThermalBox : IThermalShape
    {
        double Left { get; }
        double Top { get; }
        double Width { get; }
        double Height { get; }

        void Offset(double x, double y);
        void MoveTo(double x, double y);
        void ChangeSize(double width, double height);
        void ChangeWidth(double width);
        void ChangeHeight(double height);
        void Rotate(double angle);
    }
}
