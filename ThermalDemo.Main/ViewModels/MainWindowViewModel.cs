using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThermalDemo.Main.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _testText;
        public string TestText { get => _testText; set => SetProperty(ref _testText, value); }

        public MainWindowViewModel()
        {
            TestText = "Thermal Demo";
        }
    }
}
