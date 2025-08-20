using ColorPicker.Interfaces.Service;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace ColorPicker.ViewModels
{
    public class ColorPickerPageViewModel : BindableBase
    {
        private readonly IColorPickerService _colorPicker;

        private string _selectedColor;

        public string SelectedColor { get => _selectedColor; set => SetProperty(ref _selectedColor, value); }
        public ObservableCollection<string> ColorList { get; set; }
        public DelegateCommand<SelectionChangedEventArgs> ColorSelectionChangedCmd { get; set; }

        public ColorPickerPageViewModel(IColorPickerService colorPicker)
        {
            _colorPicker = colorPicker;

            ColorList = new ObservableCollection<string>() {
                "Red",
                "Salmon",
                "Blue",
                "Aqua",
                "Yellow",
                "Gold",
                "Green",
                "Violet",
                "Gray",
                "Black"
            };

            SelectedColor = "Yellow";
            _colorPicker.SetStrokeColor("Yellow");
            ColorSelectionChangedCmd = new DelegateCommand<SelectionChangedEventArgs>(OnColorSelectionChanged);
        }

        private void OnColorSelectionChanged(SelectionChangedEventArgs args)
        {
            _colorPicker.SetStrokeColor(_selectedColor);
        }
    }
}
