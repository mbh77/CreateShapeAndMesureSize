using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalDemo.Draw.Interfaces.Model
{
    public  interface IThermalEllipse : IThermalShape
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
