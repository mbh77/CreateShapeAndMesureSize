using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalDemo.Repository.Interfaces
{
    public interface IRepository<T>
    {
        bool Add(T data);
        bool Update(T data);
        bool Remove(string key);
        bool GetData(string key, out T data);
        int GetDataList(out T[] data);
    }
}
