using Prism.Mvvm;
using System;

namespace ThermalDemo.Draw.ViewModels.Bindable
{
    public class Box : BindableBase
    {
        private string _key;
        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private string _realArea;
        private string _stroke;
        private double _angle;

        public string Key { get => _key; set => SetProperty(ref _key, value); }
        public double Left { get => _left; set => SetProperty(ref _left, value); }
        public double Top { get => _top; set => SetProperty(ref _top, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value, () => ChangedCenterProperties()); }
        public double Height { get => _height; set => SetProperty(ref _height, value, () => ChangedCenterProperties()); }
        public string RealArea { get => string.Format("{0:0.#0} mm²", double.Parse(_realArea)); set => SetProperty(ref _realArea, value); }
        public string Stroke { get => _stroke; set => SetProperty(ref _stroke, value); }
        public double Angle { get => _angle; set => SetProperty(ref _angle, value); }
        public double CenterX { get => _width / 2; set { } }
        public double CenterY { get => _height / 2; set { } }  

        private void ChangedCenterProperties()
        {
            RaisePropertyChanged(nameof(CenterX));
            RaisePropertyChanged(nameof(CenterY));
        }
    }
}
