using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDemo.Draw.Interfaces.Model;

namespace ThermalDemo.Repository.Interfaces
{
    public interface IShapeRepository : IRepository<IThermalShape>
    {
    }
}
