using FamilyConnections.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyConnections.Core.DTO
{
    public class ConnectionDTO
    {
        public PersonDTO TargetPerson { get; set; }
        public PersonDTO RelatedPerson { get; set; }
        public RelationshipInfo Relationship { get; set; }

        public ConnectionDTO(PersonDTO target, PersonDTO related, eRel? rel)
        {
            TargetPerson = target;
            RelatedPerson = related;
            Relationship = new RelationshipInfo(rel);
        }

        public ConnectionDTO()
        {
            TargetPerson = new PersonDTO();
            RelatedPerson = new PersonDTO();
            Relationship = new RelationshipInfo();
        }

        public FlatConnection Flat
        {
            get
            {
                return new FlatConnection(TargetPerson.Id, RelatedPerson.Id, Relationship.Id);
            }
        }
    }

    public class FlatConnection
    {
        public int TargetId { get; set; }
        public int RelatedId { get; set; }
        public int RelationshipId { get; set; }

        public FlatConnection(int targetId, int relatedId, int relId)
        {
            TargetId = targetId;
            RelatedId = relatedId;
            RelationshipId = relId;
        }
    }

    public class RelationshipInfo
    {
        public RelationshipInfo() 
        {
            Id = -1;
        }

        public RelationshipInfo(eRel? type)
        {
            Type = type;
            Id = Type.HasValue ? (int)Type : -1;
        }

        public int Id { get; set; }
        public eRel? Type 
        {
            get
            {
                return (eRel)Id;
            }
            set
            {
                Id = (int)value;
            }
        }

        public void SetError(eError error)
        {
            var field = error.GetType().GetField(error.ToString());
            var attribute = (DescriptionAttribute)field?.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            Error = attribute.Description;
        }
        public string Error { get; set; }

        public static eRel Get(int relId)
        {
            return (eRel)relId;
        }
    }
}
