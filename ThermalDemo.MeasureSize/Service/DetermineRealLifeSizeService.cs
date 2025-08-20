using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Service;

namespace ThermalDemo.MeasureSize.Service
{
    internal class DetermineRealLifeSizeService : BindableBase, IDetermineRealLifeSizeService
    {
        public DetermineRealLifeSizeService()
        {
        }

        public double GetRealLifeLength(double x1, double y1, double x2, double y2, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel)
        {
            // 예외 발생등 다양한 처리 가능
            if (lengthOfPixel <= 0 || imageWidth <= 0 || imageHeight <= 0)
            {
                return 0;
            }

            var heightRatio = imageHeight / canvasHeight;
            var widthRatio = imageWidth / canvasWidth;

            var height = Math.Abs(y1 - y2) * heightRatio;
            var width = Math.Abs(x1 - x2) * widthRatio;

            return Math.Sqrt(Math.Pow(height, 2) + Math.Pow(width, 2)) * lengthOfPixel;
        }

        public double GetRealLifeBoxArea(double width, double height, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel)
        {
            // 예외 발생등 다양한 처리 가능
            if (lengthOfPixel <= 0 || imageWidth <= 0 || imageHeight <= 0)
            {
                return 0;
            }

            var heightRatio = imageHeight / canvasHeight;
            var widthRatio = imageWidth / canvasWidth;

            var realLifeWidth = width * widthRatio * lengthOfPixel;
            var realLifeHeight = height * heightRatio * lengthOfPixel;

            return realLifeWidth * realLifeHeight;
        }

        public double GetRealLifeEllipseArea(double width, double height, double imageWidth, double imageHeight, double canvasWidth, double canvasHeight, double lengthOfPixel)
        {
            // 예외 발생등 다양한 처리 가능
            if (lengthOfPixel <= 0 || imageWidth <= 0 || imageHeight <= 0)
            {
                return 0;
            }

            var heightRatio = imageHeight / canvasHeight;
            var widthRatio = imageWidth / canvasWidth;

            var realLifeWidth = width * widthRatio * lengthOfPixel;
            var realLifeHeight = height * heightRatio * lengthOfPixel;

            return (realLifeWidth / 2) * (realLifeHeight / 2) * Math.PI;
        }
    }
}
