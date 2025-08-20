using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalDemo.Media.ViewModels.Bindable
{
    public class ImageItem : BindableBase
    {
        private string _key;
        private string _path;
        private string _name;
        private double _width;
        private double _height;
        private double _displayHeight;
        private bool _expanded;

        public string Key { get => _key; set => SetProperty(ref _key, value); }
        public string Path { get => _path; set => SetProperty(ref _path, value); }
        public string ImageRegionName { get => _key + "ImageRegion";set { } }
        public string DrawingRegionName { get => _key + "DrawingRegion";set{ } }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }
        public double DisplayHeight { get => _displayHeight; set => SetProperty(ref _displayHeight, value); }
        public bool Expanded { get => _expanded; set => SetProperty(ref _expanded, value); }
    }
}
