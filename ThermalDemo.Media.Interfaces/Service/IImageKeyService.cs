using ThermalDemo.Media.Interfaces.Model;

namespace ThermalDemo.Media.Interfaces.Service
{
    public interface IImageKeyService
    {
        bool Exists(IThermalImage shape);
        string GenerateKey();
    }
}
