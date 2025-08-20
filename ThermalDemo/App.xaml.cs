using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace ThermalDemo
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Main.Views.MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Main.MainModule>();
            moduleCatalog.AddModule<Media.MediaModule>();
            moduleCatalog.AddModule<Draw.DrawModule>();
            moduleCatalog.AddModule<Repository.RepositoryModule>();
            moduleCatalog.AddModule<MeasureSize.MeasureSizeModule>();
            moduleCatalog.AddModule<ColorPicker.ColorPickerModule>();
        }
    }
}
