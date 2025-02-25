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
        void AddConnections(List<ConnectionDTO> newConnections);
        void AddConnection(ConnectionDTO newConnection);
        void AddPerson(PersonDTO newPerson);
        public List<ConnectionDTO> GetConnections(List<PersonDTO> allPersons = null);
        public List<PersonDTO> GetPersons();
    }
}
