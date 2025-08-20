using Prism.Events;
using Prism.Mvvm;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Events;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Media.Model;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Media.Service
{
    public class ImageLoadService : BindableBase, IImageLoadService
    {
        private readonly IDetermineRealLifeSizeService _realLifeSizeService;
        private readonly IImageKeyService _imageKeyService;
        private readonly IImageRepository _imageRepository;
        private readonly IEventAggregator _eventAggregator;

        public ImageLoadService(IDetermineRealLifeSizeService realLifeSizeService, IImageKeyService imageKeyService, IImageRepository imageRepository,
            IEventAggregator eventAggregator)
        {
            _realLifeSizeService = realLifeSizeService;
            _imageKeyService = imageKeyService;
            _imageRepository = imageRepository;
            _eventAggregator = eventAggregator;
        }

        public void Close()
        {
        }

        public void Load(string url)
        {
            var imagePath = url;
            var imageName = Path.GetFileNameWithoutExtension(imagePath);
            ThermalImage thermalImage = null;
            using (var image = Image.FromFile(imagePath))
            {
                var imageKey = _imageKeyService.GenerateKey();
                thermalImage = new ThermalImage(imageKey, imagePath, imageName, image.Width, image.Height);
            }

            var strLR = GetCustomExif(imagePath, 0x9c9c);
            var strDistance = GetCustomExif(imagePath, 0x9c9b);

            if (double.TryParse(strLR, out var lr) == false)
            {
                lr = 238.464;
            }

            if (double.TryParse(strDistance, out var distance) == false)
            {
                distance = 0;
            }
            thermalImage.SetPreCondition(lr, distance);

            _imageRepository.Add(thermalImage);

            _eventAggregator.GetEvent<LoadImageEvent>().Publish(new LoadImageEventArgs(LoadImageAction.Open, thermalImage));
        }

        private string GetCustomExif(string imagePath, int propertyId)
        {
            using (var image = Image.FromFile(imagePath))
            {
                var propertyItem = image.PropertyItems.ToList().Find(p => p.Id == propertyId);
                if (propertyItem != null)
                {
                    string value = System.Text.Encoding.ASCII.GetString(propertyItem.Value);
                    return value.TrimEnd('\0');
                }
            }

            return null;
        }
    }
}
