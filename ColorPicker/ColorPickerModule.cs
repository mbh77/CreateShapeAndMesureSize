using ColorPicker.Interfaces.Service;
using ColorPicker.Service;
using ColorPicker.ViewModels;
using ColorPicker.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace ColorPicker
{
    public class ColorPickerModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<ColorPickerPage, ColorPickerPageViewModel>();

            containerRegistry.RegisterForNavigation<ColorPickerPage>();
            containerRegistry.RegisterSingleton<IColorPickerService, ColorPickerService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }
    }
}
