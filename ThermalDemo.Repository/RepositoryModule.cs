using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDemo.Repository.Interfaces;

namespace ThermalDemo.Repository
{
    public class RepositoryModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IShapeRepository, ShapeInMemoryRepository>();
            containerRegistry.RegisterSingleton<IImageRepository, ImageInMemoryRepository>();
        }
    }
}
