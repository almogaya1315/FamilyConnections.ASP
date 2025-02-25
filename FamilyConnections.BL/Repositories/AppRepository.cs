using FamilyConnections.Core.Bases;
using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.BL.Repositories
{
    public class AppRepository : RepositoryBase, IRepository
    {
        private readonly FamConnContext _context;

        public AppRepository()
        {
            _context = new FamConnContext();
        }

        public List<PersonDTO> GetPersons()
        {
            return _context.GetPersons();
        }

        public List<ConnectionDTO> GetConnections(List<PersonDTO> allPersons = null)
        {
            return _context.GetConnections(allPersons);
        }

        public void AddConnections(List<ConnectionDTO> newConnections)
        {
            
        }

        public void AddConnection(ConnectionDTO newConnection)
        {
            
        }

        public void AddPerson(PersonDTO newPerson)
        {
            
        }
    }
}
