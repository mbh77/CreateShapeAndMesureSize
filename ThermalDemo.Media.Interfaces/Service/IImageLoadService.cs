namespace ThermalDemo.Media.Interfaces.Service
{
    public interface IImageLoadService
    {
        void Load(string url);
        void Close();
    }
}
