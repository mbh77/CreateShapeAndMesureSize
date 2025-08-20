using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using ThermalDemo.Draw.Interfaces.Model;
using ThermalDemo.Media.Interfaces.Model;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Repository
{
    public class ImageInMemoryRepository : IImageRepository
    {
        private ConcurrentDictionary<string, IThermalImage> _storage;

        public ImageInMemoryRepository()
        {
            _storage = new ConcurrentDictionary<string, IThermalImage>();
        }

        public bool Add(IThermalImage image)
        {
            return _storage.TryAdd(image.Key, image);
        }

        public bool Update(IThermalImage image)
        {
            if (_storage.TryGetValue(image.Key, out var oldShape))
            {
                return _storage.TryUpdate(image.Key, image, oldShape);
            }
            else
            {
                return Add(image);
            }
        }

        public bool Remove(string key)
        {
            return _storage.TryRemove(key, out var _);
        }

        public bool GetData(string key, out IThermalImage image)
        {
            return _storage.TryGetValue(key, out image);
        }

        public int GetDataList(out IThermalImage[] images)
        {
            images = _storage.Values.ToArray();
            return images.Length;
        }
    }
}
