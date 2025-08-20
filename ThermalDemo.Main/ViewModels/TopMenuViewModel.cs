using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Service;

namespace ThermalDemo.Main.ViewModels
{
    public class TopMenuViewModel : BindableBase
    {
        private readonly IThermalDrawService _drawService;
        private readonly IImageLoadService _imageLoadService;
        private readonly IDrawingModeService _drawingModeService;

        public DelegateCommand OpenFileCommand { get; set; }
        public DelegateCommand CloseFileCommand { get; set; }

        public DelegateCommand DrawLineCommand { get; set; }
        public DelegateCommand DrawBoxCommand { get; set; }
        public DelegateCommand DrawEllipseCommand { get; set; }
        public DelegateCommand SelectShapeCommand { get; set; }

        public TopMenuViewModel(IThermalDrawService drawService, IImageLoadService imageLoadService, IDrawingModeService drawingModeService)
        {
            _drawService = drawService;
            _imageLoadService = imageLoadService;

            InitDelegateCommand();
            _drawingModeService = drawingModeService;
        }

        private void InitDelegateCommand()
        {
            OpenFileCommand = new DelegateCommand(OnOpenFile);
            CloseFileCommand = new DelegateCommand(OnCloseFile);
            DrawLineCommand = new DelegateCommand(OnSetDrawingLineMode);
            DrawBoxCommand = new DelegateCommand(OnSetDrawingBoxMode);
            DrawEllipseCommand = new DelegateCommand(OnDrawingEllipseMode);
            SelectShapeCommand = new DelegateCommand(OnSelectionShapeMode);
        }

        private void OnOpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG files (*.png)|*.png|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                //_drawService.DeleteAllShapes();
                _imageLoadService.Load(filePath);
            }
        }

        private void OnCloseFile()
        {
            //_drawService.DeleteAllShapes();
            _imageLoadService.Close();
        }

        private void OnSetDrawingLineMode()
        {
            _drawingModeService.DrawingMode = DrawingMode.Line;
        }

        private void OnSetDrawingBoxMode()
        {
            _drawingModeService.DrawingMode = DrawingMode.Box;
        }

        private void OnDrawingEllipseMode()
        {
            _drawingModeService.DrawingMode = DrawingMode.Ellipse;
        }

        private void OnSelectionShapeMode()
        {
            _drawingModeService.DrawingMode = DrawingMode.Select;
        }
    }
}
