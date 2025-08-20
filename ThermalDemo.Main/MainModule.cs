using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using ThermalDemo.Main.ViewModels;
using ThermalDemo.Main.Views;

namespace ThermalDemo.Main
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<TopMenu, TopMenuViewModel>();

            containerRegistry.RegisterForNavigation<TopMenu>();
        }
    }
}
