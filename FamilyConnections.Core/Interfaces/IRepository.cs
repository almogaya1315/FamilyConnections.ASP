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
        void AddConnections(params ConnectionDTO[] newConnections);
        void AddPerson(PersonDTO newPerson);
        public List<ConnectionDTO> GetConnections(out List<FlatConnection> connectionsFlat, List<PersonDTO> allPersons = null);
        public List<PersonDTO> GetPersons();
    }
}
