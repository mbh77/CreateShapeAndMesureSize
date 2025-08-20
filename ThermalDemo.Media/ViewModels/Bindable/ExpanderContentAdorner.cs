using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ThermalDemo.Media.ViewModels.Bindable
{
    public class ExpanderContentAdorner : Adorner
    {
        private VisualCollection _adornerVisuals;

        private Thumb _bottomSize;
        private Rectangle _rectBody;

        public ExpanderContentAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _adornerVisuals = new VisualCollection(this);
            var ele = (FrameworkElement)AdornedElement;

            var bodyStyle = Application.Current.FindResource("ExpanderBottomThumbStyle") as Style;

            _bottomSize = new Thumb() { Background = Brushes.Transparent, Height = 5, Width = ele.ActualWidth,  Style = bodyStyle };
            _rectBody = new Rectangle() { Stroke = Brushes.Coral, StrokeThickness = 2 };

            _bottomSize.DragDelta += BottonSize_DeltaDrag;
            _bottomSize.Cursor = Cursors.SizeNS;

            _adornerVisuals.Add(_rectBody);
            _adornerVisuals.Add(_bottomSize);
        }

        private void BottonSize_DeltaDrag(object sender, DragDeltaEventArgs e)
        {
            if(AdornedElement is ListBoxItem boxItem && boxItem.Content is ImageItem imageItem && imageItem.Expanded == true)
            {
                imageItem.DisplayHeight = imageItem.DisplayHeight + e.VerticalChange < 100 ? 100 : imageItem.DisplayHeight + e.VerticalChange;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerVisuals[index];
        }

        protected override int VisualChildrenCount => _adornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var ele = (FrameworkElement)AdornedElement;

            double elementWidth = ele.ActualWidth;
            double elementHeight = ele.ActualHeight;

            _rectBody.Arrange(new Rect(1, 1, elementWidth -2, elementHeight -2));
            _bottomSize.Arrange(new Rect(0, elementHeight - (_bottomSize.Height / 2), _bottomSize.Width, _bottomSize.Height));

            return base.ArrangeOverride(finalSize);
        }
    }
}
