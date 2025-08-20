using System.Collections.Concurrent;
using System.Linq;
using ThermalDemo.Draw.Interfaces.Model;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Repository
{
    internal class ShapeInMemoryRepository : IShapeRepository
    {
        private ConcurrentDictionary<string, IThermalShape> _storage;

        public ShapeInMemoryRepository()
        {
            _storage = new ConcurrentDictionary<string, IThermalShape>();
        }

        public bool Add(IThermalShape shape)
        {
            return _storage.TryAdd(shape.Key, shape);
        }

        public bool GetData(string key, out IThermalShape shape)
        {
            return _storage.TryGetValue(key, out shape);
        }

        public int GetDataList(out IThermalShape[] shapes)
        {
            shapes = _storage.Values.ToArray();
            return shapes.Length;
        }

        public bool Remove(string key)
        {
            return _storage.TryRemove(key, out var _);
        }

        public bool Update(IThermalShape shape)
        {
            if (_storage.TryGetValue(shape.Key, out var oldShape))
            {
                return _storage.TryUpdate(shape.Key, shape, oldShape);
            }
            else
            {
                return Add(shape);
            }
        }
    }
}
