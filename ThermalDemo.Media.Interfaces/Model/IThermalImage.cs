using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalDemo.Media.Interfaces.Model
{
    public interface IThermalImage
    {
        string Key { get; }
        string Path { get; }
        string Name { get; }
        double Width { get; }
        double Height { get; }
        double CanvasWidth { get; set; }
        double CanvasHeight { get; set; }

        void SetPreCondition(double lr, double shootingDistance);
        double LR { get; }
        double LengthRatioConst { get; }
        double ShootingDistance { get; }
        double LengthOfPixel { get; }
    }
}
