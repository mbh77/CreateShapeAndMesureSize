using Petzold.Media2D;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ThermalDemo.Draw.Interfaces.Service;

namespace ThermalDemo.Draw.ViewModels.Bindable
{
    public class LineAdorner : Adorner
    {
        private IThermalDrawService _drawService;

        private VisualCollection _adornerVisuals;

        private Thumb _thumbStart;
        private Thumb _thumbEnd;

        public LineAdorner(UIElement adornedElement, IThermalDrawService drawService) : base(adornedElement)
        {
            _drawService = drawService;

            _adornerVisuals = new VisualCollection(this);
            var style = Application.Current.FindResource("EllipseThumbStyle") as Style;
            var bodyStyle = Application.Current.FindResource("BodyThumbStyle") as Style;

            //_thumbBody = new Thumb() { Foreground = Brushes.Coral, Background = Brushes.Transparent, Style = bodyStyle };
            _thumbStart = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbEnd = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };

            //_thumbBody.DragDelta += ThumbBody_DragDelta;
            _thumbStart.DragDelta += ThumbStart_DragDelta;
            _thumbEnd.DragDelta += ThumbEnd_DragDelta;

            _thumbStart.Cursor = Cursors.SizeAll;
            _thumbEnd.Cursor = Cursors.SizeAll;

            _thumbStart.DragCompleted += ThumbDragComplete;
            _thumbEnd.DragCompleted += ThumbDragComplete;

            //_adornerVisuals.Add(_thumbBody);
            _adornerVisuals.Add(_thumbStart);
            _adornerVisuals.Add( _thumbEnd);
        }

        private void ThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            _drawService.MoveShapeComplete(ele.Uid);
        }

        private void ThumbBody_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            _drawService.OffsetShape(ele.Uid, e.HorizontalChange, e.VerticalChange);
        }

        private void ThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is ArrowLine lineElement)
            {
                var x = lineElement.X1 + e.HorizontalChange;
                var y = lineElement.Y1 + e.VerticalChange;
                _drawService.LineMoveStartTo(lineElement.Uid, x, y);
                InvalidateVisual();
            }
        }

        private void ThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is ArrowLine lineElement)
            {
                var x = lineElement.X2 + e.HorizontalChange;
                var y = lineElement.Y2 + e.VerticalChange;
                _drawService.LineMoveEndTo(lineElement.Uid, x, y);
                InvalidateVisual();
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerVisuals[index];
        }

        protected override int VisualChildrenCount => _adornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var thumbSize = 10;
            var thumbMargin = thumbSize / 2;

            if (AdornedElement is ArrowLine lineElement)
            {
                var left = Math.Min(lineElement.X1, lineElement.X2);
                var top = Math.Min(lineElement.Y1, lineElement.Y2);
                var width = Math.Abs(lineElement.X1 - lineElement.X2) + 5;
                var height = Math.Abs(lineElement.Y1 - lineElement.Y2) + 5;

                //_thumbBody.Width = width;
                //_thumbBody.Height = height;
                //_thumbBody.Arrange(new Rect(left - 2.5, top - 2.5, width, height));

                _thumbStart.Arrange(new Rect(lineElement.X1 - thumbMargin, lineElement.Y1 - thumbMargin, thumbSize, thumbSize));
                _thumbEnd.Arrange(new Rect(lineElement.X2 - thumbMargin, lineElement.Y2 - thumbMargin, thumbSize, thumbSize));
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
