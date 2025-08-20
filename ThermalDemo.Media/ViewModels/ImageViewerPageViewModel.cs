using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Events;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Media.Service;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Media.ViewModels
{
    public class ImageViewerPageViewModel : BindableBase, INavigationAware
    {
        public IImageLoadService ImageLoadService { get; set; }
        public IImageRepository _imageRepository;
        public IEventAggregator _eventAggregator;

        private string _imageKey;
        private string _imagePath;
        private double _imageWidth;
        private double _imageHeight;
        private double _canvasWidth;
        private double _canvasHeight;

        public string ImageKey { get=> _imageKey; set => SetProperty(ref _imageKey, value); }
        public string ImagePath { get => _imagePath; set => SetProperty(ref _imagePath, value); }
        public double ImageWidth { get => _imageWidth; set => SetProperty(ref _imageWidth, value); }
        public double ImageHeight { get => _imageHeight; set => SetProperty(ref _imageHeight, value); }

        public DelegateCommand<SizeChangedEventArgs> ImageSizeChangedCmd { get; set; }
        public DelegateCommand<RoutedEventArgs> ImageControlLoadedCmd { get; set; }

        public ImageViewerPageViewModel(IImageLoadService imageLoadService, IImageRepository imageRepository, IEventAggregator eventAggregator)
        {
            ImageLoadService = imageLoadService;
            _imageRepository = imageRepository;
            _eventAggregator = eventAggregator;

            ImagePath = "pack://application:,,,/ThermalDemo.Shared;component/Resources/Square150x150Logo.scale-200.png";

            InitDelegateCommand();
        }

        private void InitDelegateCommand()
        {
            ImageSizeChangedCmd = new DelegateCommand<SizeChangedEventArgs>(OnImageSizeChanged);
            ImageControlLoadedCmd = new DelegateCommand<RoutedEventArgs>(OnImageControlLoaded);
        }

        private void OnImageControlLoaded(RoutedEventArgs args)
        {
            var image = args.Source as Image;
            _canvasWidth = image.ActualWidth;
            _canvasHeight = image.ActualHeight;

            if (_imageRepository.GetData(ImageKey, out var thermalImage))
            {
                thermalImage.CanvasWidth = _canvasWidth;
                thermalImage.CanvasHeight = _canvasHeight;
                _eventAggregator.GetEvent<LoadImageEvent>().Publish(new LoadImageEventArgs(LoadImageAction.Update, thermalImage));
            }
        }

        private void OnImageSizeChanged(SizeChangedEventArgs args)
        {
            if (_imageRepository.GetData(ImageKey, out var thermalImage))
            {
                if (args.WidthChanged)
                {
                    thermalImage.CanvasWidth = args.NewSize.Width;
                }

                if (args.HeightChanged)
                {
                    thermalImage.CanvasHeight = args.NewSize.Height;
                }

                _eventAggregator.GetEvent<LoadImageEvent>().Publish(new LoadImageEventArgs(LoadImageAction.Update, thermalImage));
                //_imageRepository.Update(thermalImage);
            }
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
