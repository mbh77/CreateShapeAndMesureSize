using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Media.Service;
using ThermalDemo.Media.ViewModels;
using ThermalDemo.Media.Views;

namespace ThermalDemo.Media
{
    public class MediaModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<ImageViewerPage, ImageViewerPageViewModel>();
            ViewModelLocationProvider.Register<ImagesPage, ImagesPageViewModel>();

            containerRegistry.RegisterForNavigation<ImageViewerPage>();
            containerRegistry.RegisterForNavigation<ImagesPage>();

            containerRegistry.RegisterSingleton<IImageLoadService, ImageLoadService>();
            containerRegistry.RegisterSingleton<IImageKeyService, ImageKeyService>();
        }
    }
}
