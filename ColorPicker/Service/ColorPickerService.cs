using ColorPicker.Interfaces.Service;

namespace ColorPicker.Service
{
    internal class ColorPickerService : IColorPickerService
    {
        private string _strokeColor;

        public void SetStrokeColor(string color)
        {
            _strokeColor = color;
        }

        public string GetStrokeColor()
        {
            return _strokeColor;
        }
    }
}
