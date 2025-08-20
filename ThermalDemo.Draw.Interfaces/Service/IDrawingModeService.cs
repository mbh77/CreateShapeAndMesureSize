using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDemo.Draw.Interfaces.Enums;

namespace ThermalDemo.Draw.Interfaces.Service
{
    public interface IDrawingModeService
    {
        DrawingMode DrawingMode { get; set; }
        void SetModeChangedEventHandler(PropertyChangedEventHandler handler);
    }
}
