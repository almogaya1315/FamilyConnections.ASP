using FamilyConnections.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Interfaces
{
    public interface IRepository
    {
        public List<PersonDTO> GetPersons();
    }
}
