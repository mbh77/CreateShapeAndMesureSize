using System.Windows;
using System.Windows.Shapes;

namespace ThermalDemo.Draw.Interfaces.Model
{
    public interface IThermalLine : IThermalShape
    {
        Point Start { get; }
        Point End { get; }
        double X1 { get; }
        double Y1 { get; }
        double X2 { get; }
        double Y2 { get; }

        void Offset(double x, double y);
        void MoveStartTo(double x, double y);
        void MoveEndTo(double x, double y);
    }
}
