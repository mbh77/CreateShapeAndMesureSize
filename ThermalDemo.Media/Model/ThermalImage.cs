using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ThermalDemo.Media.Interfaces.Model;

namespace ThermalDemo.Media.Model
{
    internal class ThermalImage : IThermalImage
    {
        private readonly string _key;
        private readonly string _path;
        private readonly string _name;
        private readonly double _width;
        private readonly double _height;
        private double _canvasWidth;
        private double _canvasHeight;

        private double _LR;
        private double _lengthRatioConst;
        private double _shootingDistance;
        private double _lengthOfPixel;

        public ThermalImage(string key, string path, string name, double width, double height)
        {
            _key = key;
            _path = path;
            _name = name;
            _width = width;
            _height = height;
        }

        public string Key => _key;
        public string Path => _path;
        public string Name => _name;
        public double Width => _width;
        public double Height => _height;
        public double CanvasWidth { get => _canvasWidth; set => _canvasWidth = value; }
        public double CanvasHeight { get => _canvasHeight; set => _canvasHeight = value; }

        public double LR
        {
            get => _LR;
            set
            {
                _LR = value;
                LengthRatioConst = 0.1 / value;
            }
        }
        public double LengthRatioConst { get => _lengthRatioConst; set => _lengthRatioConst = value; }
        public double ShootingDistance { get => _shootingDistance; set => _shootingDistance = value; }
        public double LengthOfPixel { get => _lengthOfPixel; set => _lengthOfPixel = value; }

        public void SetPreCondition(double lr, double shootingDistance)
        {
            LR = lr;
            ShootingDistance = shootingDistance;
            LengthOfPixel = _shootingDistance * _lengthRatioConst;
        }
    }
}
