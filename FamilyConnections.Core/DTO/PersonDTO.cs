using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FamilyConnections.Core.DTO
{
    public class PersonDTO //: IPerson
    {
        public PersonDTO()
        {
            Id = -1;
            //Connections = new Dictionary<PersonDTO, eRel>();
            FlatConnections = new List<FlatConnection>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthStr
        {
            get
            {
                return DateOfBirth.GetValueOrDefault().ToString("dd/MM/yyyy");
            }
            set
            {
                DateOfBirth = DateTime.Parse(value);
            }
        }
        public string PlaceOfBirth { get; set; }

        //private Dictionary<PersonDTO, eRel> _connections;
        //[JsonIgnore]
        //public Dictionary<PersonDTO, eRel> Connections
        //{
        //    get 
        //    {
        //        return _connections;
        //    }
        //    set 
        //    {
        //        _connections = value;
        //    }
        //}

        public List<FlatConnection> FlatConnections { get; set; }
        //{
        //    get 
        //    {
        //        return Connections.Select(c => new FlatConnection(Id, c.Key.Id, (int)c.Value)).ToList();
        //    }
        //    set 
        //    {
        //        var allPersons = Connections.Select(c => c.Key).ToList();
        //        Connections = value.ToDictionary(v => allPersons.Find(p => p.Id == v.RelatedId), v => RelationshipInfo.Get(v.RelationshipId));
        //    }
        //}
    }
}
