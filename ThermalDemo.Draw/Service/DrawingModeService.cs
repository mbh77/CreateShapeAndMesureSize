using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Service;

namespace ThermalDemo.Draw.Service
{
    internal class DrawingModeService : BindableBase, IDrawingModeService
    {
        private DrawingMode _drawingMode;

        public DrawingModeService()
        {
            _drawingMode = DrawingMode.Select;
        }

        public DrawingMode DrawingMode { get => _drawingMode; set => SetProperty(ref _drawingMode, value); }

        public void SetModeChangedEventHandler(PropertyChangedEventHandler handler)
        {
            PropertyChanged += handler;
        }
    }
}
