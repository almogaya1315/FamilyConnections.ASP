using FamilyConnections.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Services
{
    public class Injector : IInjector
    {
        public Injector()
        {
                
        }

        public T New<T>() where T : class, new()
        {
            return new T();
        }
    }
}
