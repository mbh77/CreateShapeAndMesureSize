using System.Windows;

namespace ThermalDemo.Draw.Interfaces.Model
{
    public interface IThermalShape
    {
        string Key { get; }
        string ImageKey { get; }

        Point LeftTop { get; }
        Point RightBottom { get; }

        double CenterX { get; }
        double CenterY { get; }
        double Angle { get; }
        string Stroke { get; }
    }
}
