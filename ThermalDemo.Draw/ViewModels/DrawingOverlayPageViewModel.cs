using ColorPicker.Interfaces.Service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Events;
using ThermalDemo.Draw.Interfaces.Model;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Events;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Draw.ViewModels
{
    public class DrawingOverlayPageViewModel : BindableBase, INavigationAware
    {
        private readonly IThermalDrawService _drawService;
        public IImageLoadService ImageLoadService { get; set; }
        private readonly IImageRepository _imageRepository;
        private readonly IDetermineRealLifeSizeService _realLifeSizeService;
        private readonly IDrawingModeService _drawingModeService;
        private readonly IColorPickerService _colorPickerService;
        private readonly IShapeRepository _shapeRepository;

        private string _imageKey;
        private string _imagePath;
        private double _imageWidth;
        private double _imageHeight;

        private bool _isDrawing;
        private bool _isDraging; // Adorner를 사용하는 이동은 Adoner 생기기 전에는 활성화 되지 않아 첫번째 선택 시 바로 이동을 위한 플래그
        private Point _dragStart;
        private Point _dragEnd;
        private Bindable.Line _dragLine;
        private Bindable.Box _dragBox;
        private Bindable.Ellipse _dragEllipse;
        private string _strokeColor;

        // View에 있는 실제 도형 아이템 컨트롤
        // View에 직접 접근하는 것은 좋지 않은 방식이나 어쩔 수 없다.
        private Grid _drawingGrid;
        private Canvas _drawingCanvas;
        private ItemsControl _lineItemsControl;
        private ItemsControl _boxItemsControl;
        private ItemsControl _ellipseItemsControl;

        public string ImageKey { get => _imageKey; set => SetProperty(ref _imageKey, value); }
        public string ImagePath { get => _imagePath; set => SetProperty(ref _imagePath, value); }
        public double ImageWidth { get => _imageWidth; set => SetProperty(ref _imageWidth, value); }
        public double ImageHeight { get => _imageHeight; set => SetProperty(ref _imageHeight, value); }

        private Dictionary<string, Adorner> _adorners;

        public ObservableCollection<Bindable.Line> Lines { get; set; }
        public ObservableCollection<Bindable.Box> Boxes { get; set; }
        public ObservableCollection<Bindable.Ellipse> Ellipses { get; set; }
        public Bindable.Line DragLine { get => _dragLine; set => SetProperty(ref _dragLine, value); }
        public Bindable.Box DragBox { get => _dragBox; set => SetProperty(ref _dragBox, value); }
        public Bindable.Ellipse DragEllipse { get => _dragEllipse; set => SetProperty(ref _dragEllipse, value); }
        public string Stroke { get => _strokeColor; set => SetProperty(ref _strokeColor, value); }

        public DelegateCommand<RoutedEventArgs> GridLoadedCmd { get; set; }
        public DelegateCommand<RoutedEventArgs> ItemsControlLoadedCmd { get; set; }
        public DelegateCommand<MouseButtonEventArgs> CanvasLButtonDownCmd { get; set; }
        public DelegateCommand<MouseButtonEventArgs> CanvasLButtonUpCmd { get; set; }
        public DelegateCommand<MouseEventArgs> CanvasMouseMoveCmd { get; set; }
        public DelegateCommand<SizeChangedEventArgs> CanvasSizeChangedCmd { get; set; }

        public DrawingOverlayPageViewModel(IEventAggregator eventAggregator, IThermalDrawService drawService, IImageLoadService imageLoadService, IImageRepository imageRepository,
            IDetermineRealLifeSizeService realLifeSizeService, IDrawingModeService drawingModeService, IColorPickerService colorPickerService, IShapeRepository shapeRepository)
        {
            _adorners = new Dictionary<string, Adorner>();

            Lines = new ObservableCollection<Bindable.Line>();
            Boxes = new ObservableCollection<Bindable.Box>();
            Ellipses = new ObservableCollection<Bindable.Ellipse>();

            _drawService = drawService;
            ImageLoadService = imageLoadService;
            _imageRepository = imageRepository;
            _realLifeSizeService = realLifeSizeService;
            _drawingModeService = drawingModeService;
            _colorPickerService = colorPickerService;
            _shapeRepository = shapeRepository;

            _isDrawing = false;
            _dragStart = new Point(0, 0);
            _dragEnd = new Point(0, 0);

            DragLine = new Bindable.Line();
            DragBox = new Bindable.Box();
            DragEllipse = new Bindable.Ellipse();

            Stroke = _colorPickerService.GetStrokeColor();

            InitPropertyChangedEventHandler();
            InitSubscribeEvents(eventAggregator);
            InitDelegateCommand();
        }

        private void InitPropertyChangedEventHandler()
        {
            _drawingModeService.SetModeChangedEventHandler(OnDrawingModeChanged);
        }

        private void OnDrawingModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DrawingMode")
            {
                switch (_drawingModeService.DrawingMode)
                {
                    case DrawingMode.Line:
                    case DrawingMode.Box:
                    case DrawingMode.Ellipse:
                        DeselectAllShapes();
                        break;
                    case DrawingMode.Select:
                    default:
                        break;
                }
            }
        }

        private void InitSubscribeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<DrawEvent>().Subscribe(UpdateShapes);
            eventAggregator.GetEvent<SelectImageEvent>().Subscribe(OnSelectImage);
        }

        private void OnSelectImage(SelectImageEventArgs args)
        {
            var image = args.Image;
            switch(args.Action)
            {
                case SelectImageAction.Select:
                    if(image.Key != ImageKey)
                    {
                        DeselectAllShapes();
                    }
                    break;
            }
        }

        private void UpdateShapes(DrawEventArgs args)
        {
            if(args.Shape?.ImageKey == ImageKey)
            {
                switch (args.Action)
                {
                    case DrawAction.Draw:
                        DrawNewShape(args.Shape);
                        break;
                    case DrawAction.Move:
                        MoveShape(args.Shape);
                        break;
                    case DrawAction.MoveComplete:
                        MeasureRealLifeSize(args.Shape);
                        break;
                    case DrawAction.Delete:
                        DeleteShape(args.Shape);
                        break;

                }
            }
        }

        private void InitDelegateCommand()
        {
            GridLoadedCmd = new DelegateCommand<RoutedEventArgs>(OnGridLoaded);
            ItemsControlLoadedCmd = new DelegateCommand<RoutedEventArgs>(OnItemsControlLoaded);
            CanvasLButtonDownCmd = new DelegateCommand<MouseButtonEventArgs>(OnCanvasLButtonDown);
            CanvasLButtonUpCmd = new DelegateCommand<MouseButtonEventArgs>(OnCanvasLButtonUp);
            CanvasMouseMoveCmd = new DelegateCommand<MouseEventArgs>(OnCanvasMouseMove);
            CanvasSizeChangedCmd = new DelegateCommand<SizeChangedEventArgs>(OnCanvasSizeChanged);
        }

        private void OnGridLoaded(RoutedEventArgs args)
        {
            _drawingGrid = args.Source as Grid;
            _drawingCanvas = FindChild<Canvas>(_drawingGrid, "DrawingCanvas");

            // GetWindow가 가끔 null을 리턴한다.
            // 사용자 정의 컨트롤이 로드 완료된 후 윈도우를 찾자
            var p = VisualTreeHelper.GetParent(_drawingGrid);
            while(!(p is UserControl))
            {
                p = LogicalTreeHelper.GetParent(_drawingGrid);
            }

            var window = Window.GetWindow(_drawingGrid);
            if(window !=  null)
            {
                window.KeyDown += OnKeyDown;
            }

            RedrawAllShapes();
        }

        private void RedrawAllShapes()
        {
            _shapeRepository.GetDataList(out var shapeList);
            var relatedShapes = shapeList.ToList().FindAll(s => s.ImageKey == ImageKey);
            if(relatedShapes != null && relatedShapes.Count > 0)
            {
                foreach(var shape in relatedShapes)
                {
                    DrawNewShape(shape);
                }
            }
        }

        private void OnItemsControlLoaded(RoutedEventArgs args)
        {
            var itemsControl = args.Source as ItemsControl;
            switch(itemsControl.Name)
            {
                case "LineItems":
                    _lineItemsControl = itemsControl;
                    break;
                case "BoxItems":
                    _boxItemsControl = itemsControl;
                    break;
                case "EllipseItems":
                    _ellipseItemsControl = itemsControl;
                    break;
            }
        }

        private void OnCanvasSizeChanged(SizeChangedEventArgs args)
        {
            var widthRatio = args.NewSize.Width / args.PreviousSize.Width;
            var heightRatio = args.NewSize.Height / args.PreviousSize.Height;
            //_drawService.ApplyRatiosToAllShapes(widthRatio, heightRatio);
            _drawService.ApplyRatiosToAllShapesInImage(ImageKey, widthRatio, heightRatio);
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            switch(args.Key)
            {
                case Key.Delete:
                    var selectedShapeKeys = _adorners.Keys.ToList();
                    foreach (var selectedKey in selectedShapeKeys)
                    {
                        _drawService.DeleteShape(selectedKey);
                    }
                    break;
            }
        }

        private void DeleteShape(IThermalShape shape)
        {
            if (shape is IThermalLine deleteThermalLine)
            {
                var deleteLIne = Lines.ToList().Find(l => l.Key == deleteThermalLine.Key);
                if(deleteLIne != null)
                {
                    Lines.Remove(deleteLIne);
                }
            }
            else if(shape is IThermalBox deleteThermalBox)
            {
                var deleteBox = Boxes.ToList().Find(b => b.Key == deleteThermalBox.Key);
                if(deleteBox != null)
                {
                    Boxes.Remove(deleteBox);
                }
            }
            else if(shape is IThermalEllipse deleteThermalEllipse)
            {
                var deleteEllipse = Ellipses.ToList().Find(e => e.Key == deleteThermalEllipse.Key);
                if(deleteEllipse != null)
                {
                    Ellipses.Remove(deleteEllipse);
                }
            }

            if(_adorners.TryGetValue(shape.Key, out var adorner))
            {
                AdornerLayer.GetAdornerLayer(_drawingCanvas).Remove(adorner);
                _adorners.Remove(shape.Key);
            }
        }

        private void MoveShape(IThermalShape shape)
        {
            if (shape is IThermalLine newThermalLine)
            {
                var updateLine = Lines.ToList().Find(l => l.Key == newThermalLine.Key);
                if(updateLine != null)
                {
                    updateLine.X1 = newThermalLine.X1;
                    updateLine.Y1 = newThermalLine.Y1;
                    updateLine.X2 = newThermalLine.X2;
                    updateLine.Y2 = newThermalLine.Y2;
                }
            }
            else if(shape is IThermalBox newThermalBox)
            {
                var updateBox = Boxes.ToList().Find(b => b.Key == newThermalBox.Key);
                if(updateBox != null)
                {
                    updateBox.Left = newThermalBox.Left;
                    updateBox.Top = newThermalBox.Top;
                    updateBox.Width = newThermalBox.Width;
                    updateBox.Height = newThermalBox.Height;
                    updateBox.Angle = newThermalBox.Angle;
                }
            }
            else if(shape is IThermalEllipse newThermalEllipse)
            {
                var updateEllipse = Ellipses.ToList().Find(e => e.Key == newThermalEllipse.Key);
                if(updateEllipse != null)
                {
                    updateEllipse.Left = newThermalEllipse.Left;
                    updateEllipse.Top = newThermalEllipse.Top;
                    updateEllipse.Width = newThermalEllipse.Width;
                    updateEllipse.Height = newThermalEllipse.Height;
                    updateEllipse.Angle = newThermalEllipse.Angle;
                }
            }
        }

        private void MeasureRealLifeSize(IThermalShape shape)
        {
            if(_imageRepository.GetData(ImageKey, out var image))
            {
                if (shape is IThermalLine newThermalLine)
                {
                    var updateLine = Lines.ToList().Find(l => l.Key == newThermalLine.Key);
                    if (updateLine != null)
                    {
                        var x1 = newThermalLine.X1;
                        var y1 = newThermalLine.Y1;
                        var x2 = newThermalLine.X2;
                        var y2 = newThermalLine.Y2;
                        var realLength = _realLifeSizeService.GetRealLifeLength(x1, y1, x2, y2, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                        updateLine.RealLength = realLength.ToString();
                    }
                }
                else if (shape is IThermalBox newThermalBox)
                {
                    var updateBox = Boxes.ToList().Find(b => b.Key == newThermalBox.Key);
                    if (updateBox != null)
                    {
                        var left = newThermalBox.Left;
                        var top = newThermalBox.Top;
                        var width = newThermalBox.Width;
                        var height = newThermalBox.Height;
                        var realArea = _realLifeSizeService.GetRealLifeBoxArea(width, height, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                        updateBox.RealArea = realArea.ToString();
                    }
                }
                else if (shape is IThermalEllipse newThermalEllipse)
                {
                    var updateEllipse = Ellipses.ToList().Find(e => e.Key == newThermalEllipse.Key);
                    if (updateEllipse != null)
                    {
                        var left = newThermalEllipse.Left;
                        var top = newThermalEllipse.Top;
                        var width = newThermalEllipse.Width;
                        var height = newThermalEllipse.Height;
                        var realArea = _realLifeSizeService.GetRealLifeEllipseArea(width, height, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                        updateEllipse.RealArea = realArea.ToString();
                    }
                }

            }
        }

        private void DrawNewShape(IThermalShape shape)
        {
            var strokeColor = _colorPickerService.GetStrokeColor();

            if (_imageRepository.GetData(ImageKey, out var image))
            {
                if (shape is IThermalLine newThermalLine)
                {
                    var x1 = newThermalLine.X1;
                    var y1 = newThermalLine.Y1;
                    var x2 = newThermalLine.X2;
                    var y2 = newThermalLine.Y2;
                    var stroke = newThermalLine.Stroke;
                    var realLength = _realLifeSizeService.GetRealLifeLength(x1, y1, x2, y2, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                    // 텍스트 표시 길이를 실제 텍스트 컨텐츠 길이로 구할 수 있는 코드 적용을 위한 임시 코드 (+)
                    PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);

                    DpiScale dpiScale = new DpiScale(96, 96);
                    if (source?.CompositionTarget != null)
                    {
                        dpiScale = new DpiScale(
                            source.CompositionTarget.TransformToDevice.M11,
                            source.CompositionTarget.TransformToDevice.M22
                        );
                    }

                    var newLIne = new Bindable.Line
                    {
                        Key = newThermalLine.Key,
                        RealLength = realLength.ToString(),
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                        Stroke = stroke,
                        TextFont = new FontFamily("Verdana"),
                        DpiScale = dpiScale,
                        FontSize = 10
                    };
                    // 텍스트 표시 길이를 실제 텍스트 컨텐츠 길이로 구할 수 있는 코드 적용을 위한 임시 코드 (-)

                    Lines.Add(newLIne);
                }
                else if (shape is IThermalBox newThermalBox)
                {
                    var left = newThermalBox.Left;
                    var top = newThermalBox.Top;
                    var width = newThermalBox.Width;
                    var height = newThermalBox.Height;
                    var angle = newThermalBox.Angle;
                    var stroke = newThermalBox.Stroke;
                    var realArea = _realLifeSizeService.GetRealLifeBoxArea(width, height, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                    var newBox = new Bindable.Box
                    {
                        Key = newThermalBox.Key,
                        RealArea = realArea.ToString(),
                        Left = left,
                        Top = top,
                        Width = width,
                        Height = height,
                        Angle = angle,
                        Stroke = stroke
                    };

                    Boxes.Add(newBox);
                }
                else if (shape is IThermalEllipse newThermalEllipse)
                {
                    var left = newThermalEllipse.Left;
                    var top = newThermalEllipse.Top;
                    var width = newThermalEllipse.Width;
                    var height = newThermalEllipse.Height;
                    var angle = newThermalEllipse.Angle;
                    var stroke = newThermalEllipse.Stroke;
                    var realArea = _realLifeSizeService.GetRealLifeEllipseArea(width, height, _imageWidth, _imageHeight, image.CanvasWidth, image.CanvasHeight, image.LengthOfPixel);

                    var newEllipse = new Bindable.Ellipse
                    {
                        Key = newThermalEllipse.Key,
                        RealArea = realArea.ToString(),
                        Left = left,
                        Top = top,
                        Width = width,
                        Height = height,
                        Angle = angle,
                        Stroke = stroke
                    };

                    Ellipses.Add(newEllipse);
                }
            }
        }

        public List<DependencyObject> hitResultsList = new List<DependencyObject>();

        private void OnCanvasLButtonDown(MouseButtonEventArgs e)
        {
            Canvas canvas = null;

            if(e.Source is Canvas sourceCanvas)
            {
                canvas = sourceCanvas;
            }
            else if(e.Source is ItemsControl)
            {
                canvas = _drawingCanvas;
            }

            if(canvas != null)
            {
                if(_drawingModeService.DrawingMode == DrawingMode.Select)
                {
                    Point pt = e.GetPosition(canvas);
                    var rt = new RectangleGeometry();
                    rt.Rect = new Rect(pt.X - 5, pt.Y - 5, 10, 10);

                    hitResultsList.Clear();

                    VisualTreeHelper.HitTest(canvas, null, new HitTestResultCallback(MyHitTestResult), new GeometryHitTestParameters(rt));

                    if (hitResultsList.Count > 0)
                    {
                        var selectedShapeList = hitResultsList.FindAll(hr => hr is Shape)?.Select(sh => sh as Shape)?.ToList();
                        DeselectAllShapes();

                        if (selectedShapeList != null && selectedShapeList.Count > 0)
                        {
                            foreach (var shape in selectedShapeList)
                            {
                                SelectShape(shape.Uid);

                                _isDraging = true;
                                _dragStart.X = pt.X;
                                _dragStart.Y = pt.Y;
                                canvas.CaptureMouse();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Stroke = _colorPickerService.GetStrokeColor();
                    _dragStart = e.GetPosition(e.Source as Canvas);

                    switch (_drawingModeService.DrawingMode)
                    {
                        case DrawingMode.Line:
                            DragLine.X1 = _dragStart.X;
                            DragLine.Y1 = _dragStart.Y;
                            DragLine.X2 = _dragStart.X;
                            DragLine.Y2 = _dragStart.Y;
                            break;
                        case DrawingMode.Box:
                            DragBox.Left = _dragStart.X;
                            DragBox.Top = _dragStart.Y;
                            DragBox.Width = 0;
                            DragBox.Height = 0;
                            break;
                        case DrawingMode.Ellipse:
                            DragEllipse.Left = _dragStart.X;
                            DragEllipse.Top = _dragStart.Y;
                            DragEllipse.Width = 0;
                            DragEllipse.Height = 0;
                            break;
                    }

                    _isDrawing = true;
                    canvas.CaptureMouse();
                }
            }
        }

        private void OnCanvasMouseMove(MouseEventArgs e)
        {
            Canvas canvas = null;

            if (e.Source is Canvas sourceCanvas)
            {
                canvas = sourceCanvas;
            }
            else if (e.Source is ItemsControl)
            {
                canvas = _drawingCanvas;
            }

            if (canvas != null)
            {
                if (_isDrawing)
                {
                    _dragEnd = e.GetPosition(canvas);

                    switch (_drawingModeService.DrawingMode)
                    {
                        case DrawingMode.Line:
                            DragLine.X2 = _dragEnd.X;
                            DragLine.Y2 = _dragEnd.Y;
                            break;
                        case DrawingMode.Box:
                            DragBox.Left = Math.Min(_dragStart.X, _dragEnd.X);
                            DragBox.Top = Math.Min(_dragStart.Y, _dragEnd.Y);
                            DragBox.Width = Math.Abs(_dragStart.X - _dragEnd.X);
                            DragBox.Height = Math.Abs(_dragStart.Y - _dragEnd.Y);
                            break;
                        case DrawingMode.Ellipse:
                            DragEllipse.Left = Math.Min(_dragStart.X, _dragEnd.X);
                            DragEllipse.Top = Math.Min(_dragStart.Y, _dragEnd.Y);
                            DragEllipse.Width = Math.Abs(_dragStart.X - _dragEnd.X);
                            DragEllipse.Height = Math.Abs(_dragStart.Y - _dragEnd.Y);
                            break;
                    }
                }
                else if(_isDraging)
                {
                    if(e.LeftButton == MouseButtonState.Pressed)
                    {
                        _dragEnd = e.GetPosition(canvas);
                        var offset_x = _dragEnd.X - _dragStart.X;
                        var offset_y = _dragEnd.Y - _dragStart.Y;

                        foreach (var key in _adorners.Keys)
                        {
                            _drawService.OffsetShape(key, offset_x, offset_y);
                        }

                        _dragStart.X = _dragEnd.X;
                        _dragStart.Y = _dragEnd.Y;
                    }
                    else
                    {
                        canvas.ReleaseMouseCapture();
                    }
                }
            }
        }

        private void OnCanvasLButtonUp(MouseButtonEventArgs e)
        {
            Canvas canvas = null;

            if (e.Source is Canvas sourceCanvas)
            {
                canvas = sourceCanvas;
            }
            else if (e.Source is ItemsControl)
            {
                canvas = _drawingCanvas;
            }

            if (canvas != null)
            {
                if (_isDrawing == true)
                {
                    var canvasRect = new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight);
                    _dragEnd = e.GetPosition(e.Source as Canvas);

                    if (canvasRect.Contains(_dragEnd) == true)
                    {
                        var stroke = _colorPickerService.GetStrokeColor();
                        switch (_drawingModeService.DrawingMode)
                        {
                            case DrawingMode.Line:
                                DragLine.X2 = _dragEnd.X;
                                DragLine.Y2 = _dragEnd.Y;
                                _drawService.DrawLine(ImageKey, DragLine.X1, DragLine.Y1, DragLine.X2, DragLine.Y2, stroke);
                                break;
                            case DrawingMode.Box:
                                DragBox.Left = Math.Min(_dragStart.X, _dragEnd.X);
                                DragBox.Top = Math.Min(_dragStart.Y, _dragEnd.Y);
                                DragBox.Width = Math.Abs(_dragStart.X - _dragEnd.X);
                                DragBox.Height = Math.Abs(_dragStart.Y - _dragEnd.Y);
                                _drawService.DrawBox(ImageKey, DragBox.Left, DragBox.Top, DragBox.Width, DragBox.Height, stroke);
                                break;
                            case DrawingMode.Ellipse:
                                DragEllipse.Left = Math.Min(_dragStart.X, _dragEnd.X);
                                DragEllipse.Top = Math.Min(_dragStart.Y, _dragEnd.Y);
                                DragEllipse.Width = Math.Abs(_dragStart.X - _dragEnd.X);
                                DragEllipse.Height = Math.Abs(_dragStart.Y - _dragEnd.Y);
                                _drawService.DrawEllipse(ImageKey, DragEllipse.Left, DragEllipse.Top, DragEllipse.Width, DragEllipse.Height, stroke);
                                break;
                        }

                        _drawingModeService.DrawingMode = DrawingMode.Select;
                    }
                }

                canvas.ReleaseMouseCapture();
            }

            ResetDrawing();
        }

        private void ResetDrawing()
        {
            _dragStart.X = 0;
            _dragStart.Y = 0;
            _dragEnd.X = 0;
            _dragEnd.Y = 0;

            DragLine.X1 = 0;
            DragLine.Y1 = 0;
            DragLine.X2 = 0;
            DragLine.Y2 = 0;

            DragBox.Left = 0;
            DragBox.Top = 0;
            DragBox.Width = 0;
            DragBox.Height = 0;

            DragEllipse.Left = 0;
            DragEllipse.Top = 0;
            DragEllipse.Width = 0;
            DragEllipse.Height = 0;

            _isDrawing = false;
            _isDraging = false;
        }

        private HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        private T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void SelectShape(string key)
        {
            var shapeDictionary = Lines.Select((l) => new { l.Key, Value = (object)l })
                        .Concat(Boxes.Select((b) => new { b.Key, Value = (object)b }))
                        .Concat(Ellipses.Select((e) => new { e.Key, Value = (object)e }))
                        .ToDictionary(item => item.Key, item => item.Value);

            if(shapeDictionary.TryGetValue(key, out var _) != true)
            {
                return;
            }

            var selectedShape = shapeDictionary[key];

            UIElement shapeItem = null;
            if(selectedShape is Bindable.Line line)
            {
                ContentPresenter cp = _lineItemsControl.ItemContainerGenerator.ContainerFromItem(line) as ContentPresenter;
                shapeItem = FindVisualChild<Petzold.Media2D.ArrowLine>(cp);
            } 
            else if(selectedShape is Bindable.Box box)
            {
                ContentPresenter cp = _boxItemsControl.ItemContainerGenerator.ContainerFromItem(box) as ContentPresenter;
                shapeItem = FindVisualChild<Rectangle>(cp);
            }
            else if(selectedShape is Bindable.Ellipse ellipse)
            {
                ContentPresenter cp = _ellipseItemsControl.ItemContainerGenerator.ContainerFromItem(ellipse) as ContentPresenter;
                shapeItem = FindVisualChild<Ellipse>(cp);
            }

            if(shapeItem != null)
            {
                if(_adorners.TryGetValue(key, out var test) == false)
                {
                    Adorner newAdorner = null;
                    if(shapeItem is Petzold.Media2D.ArrowLine arrowLine)
                    {
                        newAdorner = new Bindable.LineAdorner(arrowLine, _drawService);
                    }
                    else
                    {
                        newAdorner = new Bindable.ShapeAdorner(shapeItem, _drawService);
                    }

                    if(newAdorner != null)
                    {
                        AdornerLayer.GetAdornerLayer(_drawingCanvas).Add(newAdorner);
                        _adorners.Add(shapeItem.Uid, newAdorner);
                    }

                    //_drawService.RotateShape(key, 30);
                }
                else
                {

                }
            }
        }

        private void DeselectShape(string key)
        {

        }

        private void DeselectAllShapes()
        {
            foreach(var adorner in _adorners.Values)
            {
                AdornerLayer.GetAdornerLayer(_drawingCanvas).Remove(adorner);
            }

            _adorners.Clear();
        }

        public T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ImageKey = navigationContext.Parameters["ID"].ToString();
            ImagePath = navigationContext.Parameters["Path"].ToString();
            ImageWidth = (double)navigationContext.Parameters["Width"];
            ImageHeight = (double)navigationContext.Parameters["Height"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
