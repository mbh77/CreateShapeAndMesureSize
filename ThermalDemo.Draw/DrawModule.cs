using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.Draw.Service;
using ThermalDemo.Draw.ViewModels;
using ThermalDemo.Draw.Views;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Draw
{
    public class DrawModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<DrawingOverlayPage, DrawingOverlayPageViewModel>();

            containerRegistry.RegisterForNavigation<DrawingOverlayPage>();

            containerRegistry.RegisterSingleton<IShapeKeyService, ShapeKeyService>();
            containerRegistry.RegisterSingleton<IThermalDrawService, ThermalDrawService>();
            containerRegistry.RegisterSingleton<IDrawingModeService, DrawingModeService>();
        }
    }
}
