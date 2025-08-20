using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Events;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Events;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.MeasureSize.ViewModels
{
    public class MeasurementInfoViewModel : BindableBase
    {
        public IImageLoadService ImageLoadService { get; set; }
        public IDetermineRealLifeSizeService RealLifeSizeService { get; set; }
        public IImageRepository _imageRepository { get; set; }

        private string _imageKey;
        private double _LR = 1;
        private double _lengthRatioConst;
        private double _shootingDistance;
        private double _lengthOfPixel;

        private double _imageWidth;
        private double _imageHeight;
        private double _canvasWidth;
        private double _canvasHeight;

        public DelegateCommand ApplyPreconditionCmd { get; set; }

        public double LR { 
            get => _LR;
            set 
            {
                SetProperty(ref _LR, value);
                LengthRatioConst = 0.1 / value;
            } 
        }
        public double LengthRatioConst { get => _lengthRatioConst; set => SetProperty(ref _lengthRatioConst, value); }
        public double ShootingDistance { get => _shootingDistance; set => SetProperty(ref _shootingDistance, value); }
        public double LengthOfPixel { get => _lengthOfPixel; set => SetProperty(ref _lengthOfPixel, value); }

        public double ImageWidth { get => _imageWidth; set => SetProperty(ref _imageWidth, value); }
        public double ImageHeight { get => _imageHeight; set => SetProperty(ref _imageHeight, value); }
        public double CanvasWidth { get => _canvasWidth; set => SetProperty(ref _canvasWidth, value); }
        public double CanvasHeight { get => _canvasHeight; set => SetProperty(ref _canvasHeight, value); }

        public MeasurementInfoViewModel(IImageLoadService imageLoadService, IDetermineRealLifeSizeService realLifeSizeService, IEventAggregator eventAggregator,
            IImageRepository imageRepository)
        {
            ImageLoadService = imageLoadService;
            RealLifeSizeService = realLifeSizeService;
            _imageRepository = imageRepository;

            _imageKey = null;
            LR = 0;
            ShootingDistance = 0;
            LengthOfPixel = _shootingDistance * _lengthRatioConst;

            ImageWidth = 0;
            ImageHeight = 0;
            CanvasWidth = 0;
            CanvasHeight = 0;

            ApplyPreconditionCmd = new DelegateCommand(OnApplyPrecondition);
            InitSubscribeEvents(eventAggregator);
        }

        private void InitSubscribeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SelectImageEvent>().Subscribe(OnSelectImage);
            eventAggregator.GetEvent<LoadImageEvent>().Subscribe(OnLoadImage);
        }

        private void OnLoadImage(LoadImageEventArgs args)
        {
            var image = args.Image;
            switch(args.Action)
            {
                case LoadImageAction.Update:
                    _imageKey = image.Key;
                    LR = image.LR;
                    ShootingDistance = image.ShootingDistance;
                    LengthOfPixel = image.LengthOfPixel;

                    ImageWidth = image.Width;
                    ImageHeight = image.Height;
                    CanvasWidth = image.CanvasWidth;
                    CanvasHeight = image.CanvasHeight;
                    break;
            }
        }

        private void OnSelectImage(SelectImageEventArgs args)
        {
            var image = args.Image;

            switch (args.Action)
            {
                case SelectImageAction.Select:
                    _imageKey = image.Key;
                    LR = image.LR;
                    ShootingDistance = image.ShootingDistance;
                    LengthOfPixel = image.LengthOfPixel;

                    ImageWidth = image.Width;
                    ImageHeight = image.Height;
                    CanvasWidth = image.CanvasWidth;
                    CanvasHeight = image.CanvasHeight;
                    break;
                case SelectImageAction.Deselect:
                    _imageKey = null;
                    LR = 0;
                    ShootingDistance = 0;
                    LengthOfPixel = 0;

                    ImageWidth = 0;
                    ImageHeight = 0;
                    CanvasWidth = 0;
                    CanvasHeight = 0;
                    break;
            }
        }

        private void OnApplyPrecondition()
        {
            if (_imageKey != null)
            {
                if(_imageRepository.GetData(_imageKey, out var thermalImage))
                {
                    thermalImage.SetPreCondition(LR, ShootingDistance);
                    LengthOfPixel = thermalImage.LengthOfPixel;
                }
            }
            else
            {
                MessageBox.Show("이미지가 로드되지 않으면 사전조건을 적용할 수 없습니다.");
            }
        }
    }
}
