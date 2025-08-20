using Prism.Events;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Events;
using ThermalDemo.Draw.Interfaces.Model;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.Draw.Model;
using ThermalDemo.Draw.ViewModels.Bindable;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Draw.Service
{
    internal class ThermalDrawService : IThermalDrawService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IShapeKeyService _shapeService;
        private readonly IShapeRepository _shapeRepository;

        public ThermalDrawService(IEventAggregator eventAggregator, IShapeKeyService shapeService, IShapeRepository shapeRepository)
        {
            _eventAggregator = eventAggregator;
            _shapeService = shapeService;
            _shapeRepository = shapeRepository;
        }

        public void ApplyRatiosToAllShapes(double widthRatio, double heightRatio)
        {
            var _ = _shapeRepository.GetDataList(out var shapes);
            foreach(var shape in shapes)
            {
                if(shape is ThermalLine line)
                {
                    var x1 = line.X1 * widthRatio;
                    var y1 = line.Y1 * heightRatio;
                    var x2 = line.X2 * widthRatio;
                    var y2 = line.Y2 * heightRatio;

                    line.MoveStartTo(x1, y1);
                    line.MoveEndTo(x2, y2);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, line));
                }
                else if(shape is ThermalBox box)
                {
                    var left = box.Left * widthRatio;
                    var top = box.Top * heightRatio;
                    var width = box.Width * widthRatio;
                    var height = box.Height * heightRatio;

                    box.MoveTo(left, top);
                    box.ChangeSize(width, height);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if(shape is ThermalEllipse ellipse)
                {
                    var left = ellipse.Left * widthRatio;
                    var top = ellipse.Top * heightRatio;
                    var width = ellipse.Width * widthRatio;
                    var height = ellipse.Height * heightRatio;

                    ellipse.MoveTo(left, top);
                    ellipse.ChangeSize(width, height);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }

        public void DeleteShape(string key)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                _shapeRepository.Remove(shape.Key);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Delete, shape));
            }
        }

        public void DeleteAllShapes()
        {
            var _ = _shapeRepository.GetDataList(out var shapes);
            foreach (var shape in shapes)
            {
                _shapeRepository.Remove(shape.Key);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Delete, shape));
            }
        }

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            var key = _shapeService.GenerateKey();
            var line = new ThermalLine(key, new Point(x1, y1), new Point(x2, y2));

            if(!_shapeService.Exists(line))
            {
                _shapeRepository.Add(line);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, line));
            }
        }

        public void DrawLine(string imageKey, double x1, double y1, double x2, double y2, string stroke)
        {
            var key = _shapeService.GenerateKey();
            var line = new ThermalLine(key, imageKey, new Point(x1, y1), new Point(x2, y2), stroke);

            if (!_shapeService.Exists(line))
            {
                _shapeRepository.Add(line);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, line));
            }
        }

        public void DrawBox(double left, double top, double width, double height)
        {
            var key = _shapeService.GenerateKey();
            var box = new ThermalBox(key, left, top, width, height);

            if(!_shapeService.Exists(box))
            {
                _shapeRepository.Add(box);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, box));
            }
        }

        public void DrawBox(string imageKey, double left, double top, double width, double height, string stroke)
        {
            var key = _shapeService.GenerateKey();
            var box = new ThermalBox(key, imageKey, left, top, width, height, stroke);

            if (!_shapeService.Exists(box))
            {
                _shapeRepository.Add(box);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, box));
            }
        }

        public void DrawEllipse(double left, double top, double width, double height)
        {
            var key = _shapeService.GenerateKey();
            var ellipse = new ThermalEllipse(key, left, top, width, height);

            if (!_shapeService.Exists(ellipse))
            {
                _shapeRepository.Add(ellipse);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, ellipse));
            }
        }

        public void DrawEllipse(string imageKey, double left, double top, double width, double height, string stroke)
        {
            var key = _shapeService.GenerateKey();
            var ellipse = new ThermalEllipse(key, imageKey, left, top, width, height, stroke);

            if (!_shapeService.Exists(ellipse))
            {
                _shapeRepository.Add(ellipse);
                _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Draw, ellipse));
            }
        }

        public void RotateShapeAdditionalDegrees(string key, double addedAngle)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                var rotateAngle = shape.Angle + addedAngle;
                rotateAngle = rotateAngle % 360;

                Console.WriteLine($"rotate angle = {rotateAngle}");

                if (shape is IThermalBox box)
                {
                    box.Rotate(rotateAngle);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if(shape is IThermalEllipse ellipse)
                {
                    ellipse.Rotate(rotateAngle);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }

        public void OffsetShape(string key, double x, double y)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalBox box)
                {
                    box.Offset(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if (shape is IThermalEllipse ellipse)
                {
                    ellipse.Offset(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
                else if (shape is IThermalLine line)
                {
                    line.Offset(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, line));
                }
            }
        }

        public void MoveShapeTo(string key, double x, double y)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalBox box)
                {
                    box.MoveTo(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if (shape is IThermalEllipse ellipse)
                {
                    ellipse.MoveTo(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }

        public void ResizeShape(string key, double width, double height)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalBox box)
                {
                    box.ChangeSize(width, height);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if (shape is IThermalEllipse ellipse)
                {
                    ellipse.ChangeSize(width, height);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }

        public void MoveAndResizeShape(string key, double x, double y, double width, double height)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalBox box)
                {
                    box.MoveTo(x, y);
                    box.ChangeSize(width, height);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if (shape is IThermalEllipse ellipse)
                {
                    ellipse.MoveTo(x, y);
                    ellipse.ChangeSize(width, height);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }

        public void LineMoveStartTo(string key, double x, double y)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalLine line)
                {
                    line.MoveStartTo(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, line));
                }
            }
        }

        public void LineMoveEndTo(string key, double x, double y)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalLine line)
                {
                    line.MoveEndTo(x, y);
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, line));
                }
            }
        }

        public void MoveShapeComplete(string key)
        {
            if (_shapeRepository.GetData(key, out var shape))
            {
                if (shape is IThermalShape iShape)
                {
                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.MoveComplete, iShape));
                }
            }
        }

        public void ApplyRatiosToAllShapesInImage(string imageKey, double widthRatio, double heightRatio)
        {
            var _ = _shapeRepository.GetDataList(out var allShapes);
            var shapes = allShapes.ToList().FindAll(s => s.ImageKey == imageKey);

            foreach (var shape in shapes)
            {
                if (shape is ThermalLine line)
                {
                    var x1 = line.X1 * widthRatio;
                    var y1 = line.Y1 * heightRatio;
                    var x2 = line.X2 * widthRatio;
                    var y2 = line.Y2 * heightRatio;

                    line.MoveStartTo(x1, y1);
                    line.MoveEndTo(x2, y2);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, line));
                }
                else if (shape is ThermalBox box)
                {
                    var left = box.Left * widthRatio;
                    var top = box.Top * heightRatio;
                    var width = box.Width * widthRatio;
                    var height = box.Height * heightRatio;

                    box.MoveTo(left, top);
                    box.ChangeSize(width, height);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, box));
                }
                else if (shape is ThermalEllipse ellipse)
                {
                    var left = ellipse.Left * widthRatio;
                    var top = ellipse.Top * heightRatio;
                    var width = ellipse.Width * widthRatio;
                    var height = ellipse.Height * heightRatio;

                    ellipse.MoveTo(left, top);
                    ellipse.ChangeSize(width, height);

                    _eventAggregator.GetEvent<DrawEvent>().Publish(new DrawEventArgs(DrawAction.Move, ellipse));
                }
            }
        }
    }
}
