using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.MeasureSize.Service;
using ThermalDemo.MeasureSize.ViewModels;
using ThermalDemo.MeasureSize.Views;

namespace ThermalDemo.MeasureSize
{
    public class MeasureSizeModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<MeasurementInfo, MeasurementInfoViewModel>();

            containerRegistry.RegisterForNavigation<MeasurementInfo>();
            containerRegistry.RegisterSingleton<IDetermineRealLifeSizeService, DetermineRealLifeSizeService>();
        }
    }
}
