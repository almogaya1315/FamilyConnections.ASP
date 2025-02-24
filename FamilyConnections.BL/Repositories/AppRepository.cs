using FamilyConnections.Core.Bases;
using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.BL.Repositories
{
    public class AppRepository : RepositoryBase, IRepository
    {
        public List<PersonDTO> GetPersons()
        {
            var person1 = new PersonDTO
            {
                Id = 1,
                FullName = "Lior Matsliah",
                DateOfBirth = new DateTime(1985, 5, 23),
                PlaceOfBirth = "Israel",
            };
            var person2 = new PersonDTO
            {
                Id = 2,
                FullName = "Keren Matsliah",
                DateOfBirth = new DateTime(1984, 2, 5),
                PlaceOfBirth = "Israel",
            };
            var person3 = new PersonDTO
            {
                Id = 3,
                FullName = "Gaya Matsliah",
                DateOfBirth = new DateTime(2013, 6, 6),
                PlaceOfBirth = "Israel",
            };
            var person4 = new PersonDTO
            {
                Id = 4,
                FullName = "Almog Matsliah",
                DateOfBirth = new DateTime(2015, 3, 26),
                PlaceOfBirth = "Israel",
            };

            return new List<PersonDTO> { person1, person2, person3, person4 };
        }
    }
}
