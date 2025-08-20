using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Events;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Events;
using ThermalDemo.Media.ViewModels.Bindable;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Media.ViewModels
{
    public class ImagesPageViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IImageRepository _imageRepository;
        private IShapeRepository _shapeRepository;
        private IThermalDrawService _thermalDrawService;

        private bool _isDragging ;
        private Point _clickPosition;
        private ImageItem _dragItem;
        private ExpanderContentAdorner _prevAdorner;
        private ListBox _listBox;
        private int _selectedIndex;

        public ObservableCollection<ImageItem> Images { get; set; }
        public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }

        public DelegateCommand<RoutedEventArgs> ListBoxLoadedCmd { get; set; }
        public DelegateCommand<object> ImageItemLoadedCmd { get; set; }
        public DelegateCommand<MouseButtonEventArgs> ListBoxLButtonDownCmd { get; set; }
        public DelegateCommand<MouseEventArgs> ListBoxMouseMoveCmd { get; set; }
        public DelegateCommand<MouseButtonEventArgs> ListBoxLButtonUpCmd { get; set; }
        public DelegateCommand<DragEventArgs> ListBoxDropCmd { get; set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCmd { get; set; }
        public DelegateCommand<object> ClickCloseCmd { get; set; }

        // Realy Command 예제로 남겨둔다.
        public ICommand ImageItemLoadedCmd2 { get; set; }
        public IRegionManager RegionManager { get { return _regionManager; } set { } }

        public ImagesPageViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IImageRepository imageRepository, IShapeRepository shapeRepository,
            IThermalDrawService thermalDrawService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _imageRepository = imageRepository;
            _shapeRepository = shapeRepository;
            _thermalDrawService = thermalDrawService;

            _isDragging = false;
            _clickPosition = new Point(0, 0);
            _dragItem = null;
            _prevAdorner = null;

            Images = new ObservableCollection<ImageItem>();
            SelectedIndex = -1;

            InitPropertyChangedEventHandler();
            InitSubscribeEvents(eventAggregator);
            InitDelegateCommand();
            _shapeRepository = shapeRepository;
        }

        private void InitDelegateCommand()
        {
            ListBoxLoadedCmd = new DelegateCommand<RoutedEventArgs>(OnListBoxLoaded);
            ImageItemLoadedCmd = new DelegateCommand<object>(OnImageItemLoad);
            ListBoxLButtonDownCmd = new DelegateCommand<MouseButtonEventArgs>(OnMouseLeftButtonDown);
            ListBoxMouseMoveCmd = new DelegateCommand<MouseEventArgs> (OnMouseMove);
            ListBoxLButtonUpCmd = new DelegateCommand<MouseButtonEventArgs> (OnMouseLeftButtonUp);
            ListBoxDropCmd = new DelegateCommand<DragEventArgs>(OnDrop);
            SelectionChangedCmd = new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChanged);
            ClickCloseCmd = new DelegateCommand<object>(OnClickClose);

            // Realy Command 예제로 남겨둔다.
            ImageItemLoadedCmd2 = new RelayCommand(OnImageItemLoad2);
        }

        private void OnClickClose(object obj)
        {
            if (obj is ImageItem imageItem)
            {
                var imageRegion = _regionManager.Regions[imageItem.ImageRegionName];
                var imageView = imageRegion.Views.FirstOrDefault(v => true);
                var drawRegion = _regionManager.Regions[imageItem.DrawingRegionName];
                var drawView = drawRegion.Views.FirstOrDefault(v => true);

                if (_shapeRepository.GetDataList(out var allShapes) > 0)
                {
                    var keys = allShapes.ToList().FindAll(s => s.ImageKey == imageItem.Key).Select(item => item.Key).ToList();
                    if (keys.Count > 0)
                    {
                        foreach (var key in keys)
                        {
                            _thermalDrawService.DeleteShape(key);
                        }
                    }
                }

                imageRegion.Remove(imageView);
                drawRegion.Remove(drawView);

                Images.Remove(imageItem);

                _regionManager.Regions.Remove(imageItem.ImageRegionName);
                _regionManager.Regions.Remove(imageItem.DrawingRegionName);
            }
        }

        private void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if(args.Source is ListBox listBox)
            {
                ListBoxItem selectedItem = null;

                if(args.RemovedItems.Count > 0 && args.RemovedItems[0] != null)
                {
                    var index = Images.IndexOf(args.RemovedItems[0] as ImageItem);
                    if(index >= 0)
                    {
                        var deselectedItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(index);
                        if (_prevAdorner.AdornedElement is ListBoxItem prevItem && prevItem.Content is ImageItem prevImage && deselectedItem.Content is ImageItem desectedImage)
                        {
                            if (_prevAdorner != null && prevImage == desectedImage)
                            {
                                AdornerLayer.GetAdornerLayer(listBox).Remove(_prevAdorner);
                                _prevAdorner = null;
                            }
                        }
                    }
                }

                if(args.AddedItems.Count > 0 && args.AddedItems[0] != null)
                {
                    var imageItem = args.AddedItems[0] as ImageItem;
                    var index = Images.IndexOf(imageItem);
                    if (index >= 0)
                    {
                        selectedItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(index);

                        if(_imageRepository.GetData(imageItem.Key, out var thermalImage))
                        {
                            _eventAggregator.GetEvent<SelectImageEvent>().Publish(new SelectImageEventArgs(SelectImageAction.Select, thermalImage));
                        }
                    }
                }

                if(selectedItem != null)
                {
                    var newAdorner = new ExpanderContentAdorner(selectedItem);
                    AdornerLayer.GetAdornerLayer(listBox).Add(newAdorner);
                    _prevAdorner = newAdorner;
                }
            }
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

        // Realy Command 예제로 남겨둔다.
        private void OnImageItemLoad2(object obj)
        {

        }

        private void InitSubscribeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<LoadImageEvent>().Subscribe(OnLoadImage);
        }

        private void OnLoadImage(LoadImageEventArgs args)
        {
            switch(args.Action)
            {
                case LoadImageAction.Open:
                    SelectedIndex = -1;
                    var imageItem = new ImageItem() {
                        Key = args.Image.Key,
                        Path = args.Image.Path,
                        Name = args.Image.Name,
                        Width = args.Image.Width,
                        Height = args.Image.Height,
                        DisplayHeight = 297,
                        Expanded = true                        
                    };
                    Images.Add(imageItem);
                    break;
                case LoadImageAction.Close:
                    break;
            }
        }

        private void InitPropertyChangedEventHandler()
        {
            Images.CollectionChanged += OnImageCollectionChanged;
        }

        private void OnImageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }


        private void OnImageItemLoad(object obj)
        {
            if(obj is ImageItem imageItem)
            {
                var parameter = new NavigationParameters { 
                    { "ID", imageItem.Key },
                    { "Path", imageItem.Path },
                    { "Name", imageItem.Name },
                    { "Width", imageItem.Width },
                    { "Height", imageItem.Height }
                };

                _regionManager.Regions[imageItem.ImageRegionName].RequestNavigate("ImageViewerPage", parameter);
                _regionManager.Regions[imageItem.DrawingRegionName].RequestNavigate("DrawingOverlayPage", parameter);
            }
        }

        private void OnListBoxLoaded(RoutedEventArgs args)
        {

        }

        private void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
            if (listBoxItem != null)
            {
                _isDragging = true;
                _clickPosition = e.GetPosition(listBoxItem);
                if(listBoxItem.Content is ImageItem selectedItem)
                {
                    _dragItem = selectedItem;
                }
            }
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (listBoxItem != null && _dragItem != null)
                {
                    Point currentPosition = e.GetPosition(listBoxItem);
                    if (Math.Abs(currentPosition.X - _clickPosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(currentPosition.Y - _clickPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        DataObject dragData = new DataObject("ListBoxItem", listBoxItem);
                        DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                    }
                }
            }
        }

        private void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        private void OnDrop(DragEventArgs e)
        {
            if (_isDragging)
            {
                ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (listBoxItem != null)
                {
                    if (listBoxItem.Content is ImageItem dropItem)
                    {
                        if(_dragItem.Key != dropItem.Key)
                        {
                            var imageRegion = _regionManager.Regions[_dragItem.ImageRegionName];
                            var imageView = imageRegion.Views.FirstOrDefault(v => true);
                            var drawRegion = _regionManager.Regions[_dragItem.DrawingRegionName];
                            var drawView = drawRegion.Views.FirstOrDefault(v => true);

                            imageRegion.Remove(imageView);
                            drawRegion.Remove(drawView);

                            var dragIndex = Images.IndexOf(_dragItem);
                            var dropIndex = Images.IndexOf(dropItem);

                            Images.Remove(_dragItem);

                            _regionManager.Regions.Remove(_dragItem.ImageRegionName);
                            _regionManager.Regions.Remove(_dragItem.DrawingRegionName);

                            var insertIndex = Images.IndexOf(dropItem);
                            if (dragIndex < dropIndex)
                            {
                                Images.Insert(insertIndex + 1, _dragItem);
                            }
                            else
                            {
                                Images.Insert(insertIndex, _dragItem);
                            }
                        }
                    }
                }
            }

            _isDragging = false;
        }

        private T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }

    // Realy Command 예제로 남겨둔다.
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
