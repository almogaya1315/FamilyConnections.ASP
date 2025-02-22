using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.DTO
{
    public class PersonDTO : IPerson
    {
        public PersonDTO()
        {
            Id = -1;
            Connections = new Dictionary<IPerson, eRel>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthStr
        {
            get
            {
                return DateOfBirth.ToString("dd/MM/yyyy");
            }
            set
            {
                DateOfBirth = DateTime.Parse(value);
            }
        }
        public string PlaceOfBirth { get; set; }
        public Dictionary<IPerson, eRel> Connections { get; set; }
    }
}
