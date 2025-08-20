using Moq;
using NUnit.Framework;
using Prism.Ioc;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Media.Service;
using ThermalDemo.Repository.Interfaces;
using Unity;

namespace ThermalDemo.Media.Test.Service
{
    public class ImageLoadServiceTest
    {
        IUnityContainer container;

        [OneTimeSetUp]
        public void ImageLoadServiceTestSetup()
        {
            container = new UnityContainer();

            var mockDetermineRealSize = new Mock<IDetermineRealLifeSizeService>();
            var mockImageRepository = new Mock<IImageRepository>();
            container.RegisterInstance(mockDetermineRealSize.Object);
            container.RegisterInstance(mockImageRepository.Object);
            container.RegisterSingleton<IImageLoadService, ImageLoadService>();
        }

        [Test]
        public void ResolveImageLoadService()
        {
            var imageLoadService = container.Resolve<IImageLoadService>();

            Assert.IsNotNull(imageLoadService);
        }

        [Test]
        public void LoadImage()
        {
            var imageLoadService = container.Resolve<IImageLoadService>();

            Assert.IsNotNull(imageLoadService);

            imageLoadService.Load("D:\\work_data\\Thermo\\measure_real_size_sample\\MSCM-000E8E926924_20230602_100802_400mm.png");
        }
    }
}
