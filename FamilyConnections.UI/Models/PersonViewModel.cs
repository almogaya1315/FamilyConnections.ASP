using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;

namespace FamilyConnections.UI.Models
{
    public class PersonViewModel : IPerson
    {
        private PersonDTO _person;

        public PersonViewModel()
        {
            _person = new PersonDTO();

            //Id = -1;
            //Connections = new Dictionary<IPerson, eRel>();
        }

        public int Id
        {
            get
            {
                return _person.Id;
            }
            set
            {
                _person.Id = value;
            }
        }
        public string FullName
        {
            get
            {
                return _person.FullName;
            }
            set
            {
                _person.FullName = value;
            }
        }

        public DateTime DateOfBirth 
        {
            get 
            {
                return _person.DateOfBirth;
            }
            set 
            {
                _person.DateOfBirth = value;
            }
        }
        public string DateOfBirthStr
        {
            get
            {
                return _person.DateOfBirthStr;
            }
            set
            {
                _person.DateOfBirthStr = value;
            }
        }
        public string PlaceOfBirth 
        {
            get
            {
                return _person.PlaceOfBirth;
            }
            set 
            {
                _person.PlaceOfBirth = value;
            } 
        }
        public Dictionary<IPerson, eRel> Connections 
        {
            get 
            { 
                return _person.Connections;
            }
            set 
            {
                _person.Connections = value;
            }
        }
    }
}
