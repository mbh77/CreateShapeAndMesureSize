using Prism.Mvvm;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Shapes;
using ThermalDemo.Draw.Interfaces.Model;

namespace ThermalDemo.Draw.Model
{
    internal class ThermalLine : IThermalLine
    {
        private readonly string _key;
        private readonly string _imageKey;
        private Point _start;
        private Point _end;
        private double _angle;
        private string _stroke;

        public string Key => _key;
        public string ImageKey => _imageKey;
        public Point Start => _start;
        public Point End => _end;
        public double X1 => _start.X;
        public double Y1 => _start.Y;
        public double X2 => _end.X;
        public double Y2 => _end.Y;
        public string Stroke => _stroke;

        public Point LeftTop 
        {
            get
            {
                return new Point(Math.Min(X1, X2), Math.Min(Y1, Y2));
            }
        }

        public Point RightBottom
        {
            get
            {
                return new Point(Math.Max(X1, X2), Math.Max(Y1, Y2));
            }
        }

        public ThermalLine(string key, Point start, Point end)
        {
            _key = key;
            _imageKey = "Unknown";
            _start = start;
            _end = end;
            _angle = 0;
        }

        public ThermalLine(string key, string imageKey, Point start, Point end, string stroke)
        {
            _key = key;
            _imageKey = imageKey;
            _start = start;
            _end = end;
            _angle = 0;
            _stroke = stroke;
        }

        public void Offset(double x, double y)
        {
            _start.Offset(x, y);
            _end.Offset(x, y);
        }

        public void MoveStartTo(double x, double y)
        {
            _start.X = x;
            _start.Y = y;
        }

        public void MoveEndTo(double x, double y)
        {
            _end.X = x;
            _end.Y = y;
        }

        public double CenterX => (RightBottom.X - LeftTop.X) / 2;
        public double CenterY => (RightBottom.Y - LeftTop.Y) / 2;
        public double Angle => _angle;
    }
}
