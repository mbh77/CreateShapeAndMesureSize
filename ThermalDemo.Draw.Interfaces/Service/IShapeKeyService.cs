using ThermalDemo.Draw.Interfaces.Model;

namespace ThermalDemo.Draw.Interfaces.Service
{
    public interface IShapeKeyService
    {
        bool Exists(IThermalShape shape);
        string GenerateKey();
    }
}
