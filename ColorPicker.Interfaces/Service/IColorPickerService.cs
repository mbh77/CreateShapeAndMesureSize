using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker.Interfaces.Service
{
    public interface IColorPickerService
    {
        void SetStrokeColor(string color);
        string GetStrokeColor();
    }
}
