using Moq;
using NUnit.Framework;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using ThermalDemo.MeasureSize.Interfaces.Service;
using ThermalDemo.Media.Interfaces.Model;
using ThermalDemo.Media.Interfaces.Service;
using ThermalDemo.Media.Service;
using ThermalDemo.Repository;
using ThermalDemo.Repository.Interfaces;
using Unity;
using Unity.Registration;

namespace ThermalDemo.Media.Test.Service
{
    public class ImageKeyServiceTest
    {
        IUnityContainer _container;
        IModule _mediaModule;

        [OneTimeSetUp]
        public void ImageKeyServiceTestSetup()
        {
            _container = new UnityContainer();
            _container.RegisterSingleton<IImageKeyService, ImageKeyService>();
            _container.RegisterSingleton<IImageRepository, ImageInMemoryRepository>();

            //_mediaModule = new MediaModule();
            //_mediaModule.RegisterTypes(new MyContainer());
        }

        [Test]
        public void ExistsTest()
        {
            var imageKeyService = _container.Resolve<IImageKeyService>();
            var imageRepository = _container.Resolve<IImageRepository>();
            Assert.IsNotNull(imageKeyService);
        }
    }

    public class MyContainer : IContainerRegistry
    {
        private readonly IUnityContainer _container;

        public MyContainer()
        {
            _container = new UnityContainer();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name = null)
        {
            _container.RegisterInstance(type, instance);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name = null)
        {
            _container.RegisterSingleton(from, to, name);
            return this;
        }

        // Implement IDisposable if your custom container needs cleanup logic
        public void Dispose()
        {
            _container.Dispose();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
        }
    }
}
