using Microsoft.Win32.SafeHandles;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Xml.Linq;
using ThermalDemo.Draw.Interfaces.Service;
using static Unity.Storage.RegistrationSet;

namespace ThermalDemo.Draw.ViewModels.Bindable
{
    public class ShapeAdorner : Adorner
    {
        private IThermalDrawService _drawService;

        private VisualCollection _adornerVisuals;

        private Thumb _thumbLT;
        private Thumb _thumbCT;
        private Thumb _thumbRT;
        private Thumb _thumbLC;
        private Thumb _thumbRC;
        private Thumb _thumbLB;
        private Thumb _thumbCB;
        private Thumb _thumbRB;
        private Thumb _thumbHead;
        private Rectangle _rectBody;
        private System.Windows.Shapes.Line _lineHead;

        private Cursor _rotateCursor;

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public ShapeAdorner(UIElement adornedElement, IThermalDrawService drawService) : base(adornedElement)
        {
            _drawService = drawService;
            _adornerVisuals = new VisualCollection(this);

            var style = Application.Current.FindResource("EllipseThumbStyle") as Style;
            var bodyStyle = Application.Current.FindResource("BodyThumbStyle") as Style;

            _thumbLT = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbCT = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbRT = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbLC = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbRC = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbLB = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbCB = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbRB = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _thumbHead = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10, Style = style };
            _rectBody = new Rectangle() { Stroke = Brushes.Coral, StrokeThickness = 2, StrokeDashArray = { 3, 2 } };
            _lineHead = new System.Windows.Shapes.Line() { Stroke = Brushes.Coral, StrokeThickness = 2 };

            _thumbLT.DragDelta += ThumbLT_DragDelta;
            _thumbCT.DragDelta += ThumbCT_DragDelta;
            _thumbRT.DragDelta += ThumbRT_DragDelta;
            _thumbLC.DragDelta += ThumbLC_DragDelta;
            _thumbRC.DragDelta += ThumbRC_DragDelta;
            _thumbLB.DragDelta += ThumbLB_DragDelta;
            _thumbCB.DragDelta += ThumbCB_DragDelta;
            _thumbRB.DragDelta += ThumbRB_DragDelta;
            _thumbHead.DragDelta += ThumbHead_DragDelta;

            _thumbLT.DragCompleted += ThumbDragComplete;
            _thumbCT.DragCompleted += ThumbDragComplete;
            _thumbRT.DragCompleted += ThumbDragComplete;
            _thumbLC.DragCompleted += ThumbDragComplete;
            _thumbRC.DragCompleted += ThumbDragComplete;
            _thumbLB.DragCompleted += ThumbDragComplete;
            _thumbCB.DragCompleted += ThumbDragComplete;
            _thumbRB.DragCompleted += ThumbDragComplete;

            _adornerVisuals.Add(_lineHead);
            _adornerVisuals.Add(_rectBody);
            _adornerVisuals.Add(_thumbLT);
            _adornerVisuals.Add(_thumbCT);
            _adornerVisuals.Add(_thumbRT);
            _adornerVisuals.Add(_thumbLC);
            _adornerVisuals.Add(_thumbRC);
            _adornerVisuals.Add(_thumbLB);
            _adornerVisuals.Add(_thumbCB);
            _adornerVisuals.Add(_thumbRB);
            _adornerVisuals.Add(_thumbHead);

            // 회전을 커서 변경
            RotateTransform rotation = adornedElement.RenderTransform as RotateTransform;
            if (rotation != null)
            {
                SetThumbCursor(rotation.Angle);
            }
        }

        private void ThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            _drawService.MoveShapeComplete(ele.Uid);
        }

        private void SetThumbCursor(double angle)
        {
            if(_rotateCursor == null)
            {
                string ImagePath = "pack://application:,,,/ThermalDemo.Shared;component/Resources/rotateCursor.cur";
                StreamResourceInfo streamResource = Application.GetResourceStream(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(streamResource.Stream);

                IntPtr ptr = bmp.GetHicon();
                IconInfo tmp = new IconInfo();
                GetIconInfo(ptr, ref tmp);
                tmp.xHotspot = 12;
                tmp.yHotspot = 12;
                tmp.fIcon = false;
                ptr = CreateIconIndirect(ref tmp);
                var winformCursor = new System.Windows.Forms.Cursor(ptr);

                SafeFileHandle panHandle = new SafeFileHandle(winformCursor.Handle, false);
                _rotateCursor = CursorInteropHelper.Create(panHandle);
                _thumbHead.Cursor = _rotateCursor;
            }

            // 음수 각도가 나오면 양수 각도로 변경
            if(angle < 0)
            {
                angle = angle + 360; 
            }


            if (angle > 315 || angle <= 45 || (angle > 135 && angle <= 225))
            {
                _thumbLT.Cursor = Cursors.SizeNWSE;
                _thumbCT.Cursor = Cursors.SizeNS;
                _thumbRT.Cursor = Cursors.SizeNESW;
                _thumbLC.Cursor = Cursors.SizeWE;
                _thumbRC.Cursor = Cursors.SizeWE;
                _thumbLB.Cursor = Cursors.SizeNESW;
                _thumbCB.Cursor = Cursors.SizeNS;
                _thumbRB.Cursor = Cursors.SizeNWSE;
            }
            else if((angle > 45 && angle <= 135) || (angle > 225 && angle <= 315))
            {
                _thumbLT.Cursor = Cursors.SizeNESW;
                _thumbCT.Cursor = Cursors.SizeWE;
                _thumbRT.Cursor = Cursors.SizeNWSE;
                _thumbLC.Cursor = Cursors.SizeNS;
                _thumbRC.Cursor = Cursors.SizeNS;
                _thumbLB.Cursor = Cursors.SizeNWSE;
                _thumbCB.Cursor = Cursors.SizeWE;
                _thumbRB.Cursor = Cursors.SizeNESW;
            }
            else
            {
                _thumbLT.Cursor = Cursors.SizeNWSE;
                _thumbCT.Cursor = Cursors.SizeNS;
                _thumbRT.Cursor = Cursors.SizeNESW;
                _thumbLC.Cursor = Cursors.SizeWE;
                _thumbRC.Cursor = Cursors.SizeWE;
                _thumbLB.Cursor = Cursors.SizeNESW;
                _thumbCB.Cursor = Cursors.SizeNS;
                _thumbRB.Cursor = Cursors.SizeNWSE;
            }
        }

        private void ThumCursorRotation(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void ThumbCursorReset(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ThumbLT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = e.HorizontalChange;
            double y = e.VerticalChange;
            double width = ele.Width - e.HorizontalChange < 0 ? 0 : ele.Width - e.HorizontalChange;
            double height = ele.Height - e.VerticalChange < 0 ? 0 : ele.Height - e.VerticalChange;

            if (ele.Width > 0 && ele.Width - e.HorizontalChange < 0)
            {
                x = ele.Width;
            }

            if (ele.Height > 0 && ele.Height - e.VerticalChange < 0)
            {
                y = ele.Height;
            }

            if(ele.Width == 0 && x > 0)
            {
                x = 0;
            }

            if(ele.Height == 0 && y > 0)
            {
                y = 0;
            }

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbCT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = 0;
            double y = e.VerticalChange;
            double width = ele.Width;
            double height = ele.Height - e.VerticalChange < 0 ? 0 : ele.Height - e.VerticalChange;

            if (ele.Height > 0 && ele.Height - e.VerticalChange < 0)
            {
                y = ele.Height;
            }

            if (ele.Height == 0 && y > 0)
            {
                y = 0;
            }

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbRT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = 0;
            double y = e.VerticalChange;
            double width = ele.Width + e.HorizontalChange < 0 ? 0 : ele.Width + e.HorizontalChange;
            double height = ele.Height - e.VerticalChange < 0 ? 0 : ele.Height - e.VerticalChange;

            if (ele.Height > 0 && ele.Height - e.VerticalChange < 0)
            {
                y = ele.Height;
            }

            if (ele.Height == 0 && y > 0)
            {
                y = 0;
            }

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbLC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = e.HorizontalChange;
            double y = 0;
            double width = ele.Width - e.HorizontalChange < 0 ? 0 : ele.Width - e.HorizontalChange;
            double height = ele.Height;

            if (ele.Width > 0 && ele.Width - e.HorizontalChange < 0)
            {
                x = ele.Width;
            }

            if (ele.Width == 0 && x > 0)
            {
                x = 0;
            }

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbRC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = 0;
            double y = 0;
            double width = ele.Width + e.HorizontalChange < 0 ? 0 : ele.Width + e.HorizontalChange;
            double height = ele.Height;

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbLB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double x = e.HorizontalChange;
            double y = 0;
            double width = ele.Width - e.HorizontalChange < 0 ? 0 : ele.Width - e.HorizontalChange;
            double height = ele.Height + e.VerticalChange < 0 ? 0 : ele.Height + e.VerticalChange;

            if (ele.Width > 0 && ele.Width - e.HorizontalChange < 0)
            {
                x = ele.Width;
            }

            if (ele.Width == 0 && x > 0)
            {
                x = 0;
            }

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x + x, lt_y + y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbCB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double width = ele.Width;
            double height = ele.Height + e.VerticalChange < 0 ? 0 : ele.Height + e.VerticalChange;

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x, lt_y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        private void ThumbRB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double width = ele.Width + e.HorizontalChange < 0 ? 0 : ele.Width + e.HorizontalChange;
            double height = ele.Height + e.VerticalChange < 0 ? 0 : ele.Height + e.VerticalChange;

            // 회전을 고려하여 좌표를 재 조정
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            double angle = 0;

            if (rotation != null)
            {
                angle = rotation.Angle;
            }
            double lt_x = Canvas.GetLeft(ele);
            double lt_y = Canvas.GetTop(ele);

            var leftTop = GetAdjustingPointWithRotation(angle, new Point(lt_x, lt_y), new Point(lt_x, lt_y), ele.Width, ele.Height, width, height);

            // 도형을 다시 그린다.
            _drawService.MoveAndResizeShape(ele.Uid, leftTop.X, leftTop.Y, width, height);
        }

        // 회전된 도형에서 사이즈 조정했을 때 조정된 사이즈의 중심 좌표를 기준으로 회전되기 이전의 좌표 값을 구한다.
        private Point GetAdjustingPointWithRotation(double angle, Point origin_lt, Point resize_lt, double originWidth, double originHeight, double chnagedWidht, double changedHeight)
        {
            double lt_x = origin_lt.X;
            double lt_y = origin_lt.Y;
            double cx = originWidth / 2 + lt_x;
            double cy = originHeight / 2 + lt_y;

            // 구하려고 하는 좌표의 회전된 실제 좌표를 구한다.
            var rotatedResizePoint = RotatePoint(angle, cx, cy, resize_lt.X, resize_lt.Y);

            // 구하려고 하는 좌표를 기준으로 사이즈 조정했을 때 회전된 중심 좌표를 구한다.
            double rs_cx = (chnagedWidht / 2) + resize_lt.X;
            double rs_cy = (changedHeight / 2) + resize_lt.Y;
            var rotated_ct = RotatePoint(angle, cx, cy, rs_cx, rs_cy);

            // 사이즈 변경된 중심 좌표를 기준으로 회전 전 각도로 회전된 좌표 값
            var newLeftTop = RotatePoint(360 - angle, rotated_ct.X, rotated_ct.Y, rotatedResizePoint.X, rotatedResizePoint.Y);

            return newLeftTop;
        }

        private Point RotatePoint(double angleInDegrees, double cx, double cy, double x, double y)
        {
            // Convert angle from degrees to radians
            double angleInRadians = angleInDegrees * Math.PI / 180.0;
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);

            // Translate the point so that the center becomes the origin (0, 0)
            double translatedX = x - cx;
            double translatedY = y - cy;

            // Apply rotation formula
            double rotatedX = translatedX * cosTheta - translatedY * sinTheta;
            double rotatedY = translatedX * sinTheta + translatedY * cosTheta;

            // Translate the point back to its original position
            x = rotatedX + cx;
            y = rotatedY + cy;

            return new Point(x, y);
        }

        private void ThumbHead_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;

            double headMargin = 30;
            var center = new Point(ele.Width/2, ele.Height/2);

            double deltaX = ele.Height / 2 + headMargin;
            double deltaY = 0;

            deltaX -= e.VerticalChange;
            deltaY += e.HorizontalChange;


            double angle = Math.Atan2(deltaY, deltaX);

            // Convert to degrees.
            angle = angle * 180 / Math.PI;

            Console.WriteLine($"deltaX = {deltaX}, deltaY = {deltaY}, delta angle = {angle}");

            _drawService.RotateShapeAdditionalDegrees(ele.Uid, angle);
            InvalidateVisual();

            // 회전을 커서 변경
            RotateTransform rotation = ele.RenderTransform as RotateTransform;
            if (rotation != null)
            {
                SetThumbCursor(angle + rotation.Angle);
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
            double elementWidth = ele.DesiredSize.Width;
            double elementHeight = ele.DesiredSize.Height;
            double thumbSize = 10;
            double thumbMargin = thumbSize/2;
            double headMargin = 30;

            _lineHead.X1 = 0;
            _lineHead.Y1 = 0;
            _lineHead.X2 = 0;
            _lineHead.Y2 = 30;
            _lineHead.Arrange(new Rect(elementWidth / 2, -headMargin, thumbMargin, headMargin));
            _rectBody.Arrange(new Rect(-2.5, -2.5, elementWidth + 5, elementHeight + 5));
            _thumbLT.Arrange(new Rect(-thumbMargin, -thumbMargin, thumbSize, thumbSize));
            _thumbCT.Arrange(new Rect(elementWidth / 2 - thumbMargin, -thumbMargin, thumbSize, thumbSize));
            _thumbRT.Arrange(new Rect(elementWidth - thumbMargin, -thumbMargin, thumbSize, thumbSize));
            _thumbLC.Arrange(new Rect(-thumbMargin, elementHeight / 2 - thumbMargin, thumbSize, thumbSize));
            _thumbRC.Arrange(new Rect(elementWidth - thumbMargin, elementHeight / 2 - thumbMargin, thumbSize, thumbSize));
            _thumbLB.Arrange(new Rect(-thumbMargin, elementHeight - thumbMargin, thumbSize, thumbSize));
            _thumbCB.Arrange(new Rect(elementWidth / 2 - thumbMargin, elementHeight - thumbMargin, thumbSize, thumbSize));
            _thumbRB.Arrange(new Rect(elementWidth - thumbMargin, elementHeight - thumbMargin, thumbSize, thumbSize));
            _thumbHead.Arrange(new Rect(elementWidth / 2 - thumbMargin, -headMargin, thumbSize, thumbSize));



            return base.ArrangeOverride(finalSize);
        }
    }
}
