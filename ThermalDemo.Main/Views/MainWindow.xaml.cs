using Prism.Regions;
using System.Windows;

namespace ThermalDemo.Main.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IRegionManager _regionManager;

        private bool _isFristWindowActivated = true;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            if (_isFristWindowActivated)
            {
                _isFristWindowActivated = false;
                _regionManager.Regions["TopMenuRegion"].RequestNavigate("TopMenu");
                _regionManager.Regions["LeftMenuRegion"].RequestNavigate("MeasurementInfo");
                _regionManager.Regions["OptionRegion"].RequestNavigate("ColorPickerPage");

                _regionManager.Regions["ImagesRegion"].RequestNavigate("ImagesPage");
            }
        }
    }
}
