using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Interfaces
{
    public interface IInjector
    {
        public T New<T>() where T : class, new();
    }
}
