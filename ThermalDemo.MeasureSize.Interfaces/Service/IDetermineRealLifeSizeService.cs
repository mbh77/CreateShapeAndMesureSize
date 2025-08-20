using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalDemo.MeasureSize.Interfaces.Service
{
    public interface IDetermineRealLifeSizeService
    {
        double GetRealLifeLength(double x1, double y1, double x2, double y2, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel);
        double GetRealLifeBoxArea(double width, double height, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel);
        double GetRealLifeEllipseArea(double width, double height, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel);
    }
}
