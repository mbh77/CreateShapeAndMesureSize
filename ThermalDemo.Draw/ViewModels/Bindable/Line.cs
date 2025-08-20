using Prism.Mvvm;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace ThermalDemo.Draw.ViewModels.Bindable
{
    public class Line : BindableBase
    {
        private string _key;
        private string _realLength;
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;
        private string _stroke;
        private double _angle;

        public string Key { get => _key; set => SetProperty(ref _key, value); }
        public string Name { get => _key; set => SetProperty(ref _key, value); }
        public string RealLength { get => string.Format("{0:0.#0} mm", double.Parse(_realLength)); set => SetProperty(ref _realLength, value); }
        public double X1 { get => _x1; set => SetProperty(ref _x1, value, () => ChangedLabelProperties()); }
        public double Y1 { get => _y1; set => SetProperty(ref _y1, value, () => ChangedLabelProperties()); }
        public double X2 { get => _x2; set => SetProperty(ref _x2, value, () => ChangedLabelProperties()); }
        public double Y2 { get => _y2; set => SetProperty(ref _y2, value, () => ChangedLabelProperties()); }
        public string Stroke { get => _stroke; set => SetProperty(ref _stroke, value); }
        public double Angle { get => _angle; set => SetProperty(ref _angle, value); }

        public double LabelAngle
        {
            get => (GetLabelAngle() / Math.PI * 180) + 270;
            set { }
        }

        public Point LabelLeftTop 
        { 
            get => GetLableLeftTop(25); 
            set { } 
        }

        public FontFamily TextFont { get; set; }
        public DpiScale DpiScale { get; set; }
        public int FontSize { get; set; }

        public double LineLength 
        { 
            get => GetLineLength();
            set { } 
        }

        public double TextLength
        {
            get => GetTextLength();
            set { }
        }

        private double GetTextLength()
        {
            // Create the initial formatted text string.
            FormattedText formattedText = new FormattedText
            (
                RealLength,
                new CultureInfo("en-US"),
                FlowDirection.LeftToRight,
                new Typeface(TextFont, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                FontSize,
                Brushes.Black,
                DpiScale.PixelsPerDip
            );

            return Math.Max(formattedText.Width, LineLength);
        }

        private void ChangedLabelProperties()
        {
            RaisePropertyChanged(nameof(LabelAngle));
            RaisePropertyChanged(nameof(LabelLeftTop));
            RaisePropertyChanged(nameof(LineLength));
            RaisePropertyChanged(nameof(TextLength));
        }

        private double GetLineLength()
        {
            var height = Math.Abs(Y1 - Y2);
            var width = Math.Abs(X1 - X2);

            return Math.Sqrt(Math.Pow(height,2) + Math.Pow(width, 2));
        }

        private double GetLabelAngle()
        {
            var height = Y1 - Y2; // 윈도우 좌표는 아래로 갈수록 수가 커진다.
            var width = X2 - X1;

            var radian = Math.Atan2(width, height);

            return radian;
        }

        private Point GetLableLeftTop(double offset)
        {
            var radian = GetLabelAngle();
            radian = (radian + Math.PI) % (2 * Math.PI);

            var x = Math.Cos(radian) * (offset);
            var y = Math.Sin(radian) * (offset);

            var label_left = X1 + x; 
            var label_top = Y1 + y;

            return new Point(label_left, label_top);
        }
    }
}
