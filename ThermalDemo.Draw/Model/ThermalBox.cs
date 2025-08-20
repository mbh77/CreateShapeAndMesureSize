using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ThermalDemo.Draw.Interfaces.Model;

namespace ThermalDemo.Draw.Model
{
    internal class ThermalBox : IThermalBox
    {
        private readonly string _key;
        private readonly string _imageKey;
        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private double _angle;
        private string _stroke;

        public string Key => _key;
        public string ImageKey => _imageKey;
        public double Left => _left;
        public double Top => _top;
        public double Width => _width;
        public double Height => _height;

        public Point LeftTop => new Point(_left, _top);
        public Point RightBottom => new Point(_left + _width, _top + _height);
        public double CenterX => _width / 2;
        public double CenterY => _height / 2;
        public double Angle => _angle;
        public string Stroke => _stroke;

        public ThermalBox(string key, double left, double top, double width, double height)
        {
            _key = key;
            _imageKey = "Unknown";
            _left = left;
            _top = top;
            _width = width;
            _height = height;
            _angle = 0;
        }

        public ThermalBox(string key, string imageKey, double left, double top, double width, double height, string stroke)
        {
            _key = key;
            _imageKey = imageKey;
            _left = left;
            _top = top;
            _width = width;
            _height = height;
            _angle = 0;
            _stroke = stroke;
        }

        public void ChangeSize(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public void ChangeWidth(double width)
        {
            _width = width;
        }

        public void ChangeHeight(double height)
        {
            _height = height;
        }

        public void MoveTo(double x, double y)
        {
            _left = x;
            _top = y;
        }

        public void Offset(double x, double y)
        {
            _left += x;
            _top += y;
        }

        public void Rotate(double angle)
        {
            _angle = angle;
        }
    }
}
