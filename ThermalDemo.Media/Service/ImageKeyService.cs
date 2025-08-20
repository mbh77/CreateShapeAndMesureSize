
using ThermalDemo.Media.Interfaces.Model;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Media.Service
{
    public class ImageKeyService : IImageKeyService
    {
        private int _id;
        private readonly IImageRepository _imageRepository;

        public ImageKeyService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
            _id = 0;            
        }

        public bool Exists(IThermalImage image)
        {
            return _imageRepository.GetData(image.Key, out var _);
        }

        public string GenerateKey()
        {
            string key = $"{_id}";

            while (_imageRepository.GetData(key, out var _))
            {
                _id++;
                key = $"{_id}";
            }

            return key;
        }
    }
}
