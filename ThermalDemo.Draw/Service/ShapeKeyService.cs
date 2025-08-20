using ThermalDemo.Draw.Interfaces.Model;
using ThermalDemo.Draw.Interfaces.Service;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Draw.Service
{
    internal class ShapeKeyService : IShapeKeyService
    {
        private int _id;

        private readonly IShapeRepository _shapeRepository;
        public ShapeKeyService(IShapeRepository shapeRepository)
        {
            _shapeRepository = shapeRepository;
            _id = 0;
        }

        public bool Exists(IThermalShape shape)
        {
            return _shapeRepository.GetData(shape.Key, out var _);
        }

        public string GenerateKey()
        {
            string key = $"{_id}";

            while(_shapeRepository.GetData(key, out var _))
            {
                _id++;
                key = $"{_id}";
            }

            return key;
        }
    }
}
