using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
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
            Flat = new FlatConnection(TargetPerson.Id, RelatedPerson.Id, Relationship.Id);
        }
        public ConnectionDTO()
        {
            TargetPerson = new PersonDTO();
            RelatedPerson = new PersonDTO();
            Relationship = new RelationshipInfo();
        }
        private FlatConnection _flat;
        public FlatConnection Flat
        {
            get
            {
                return _flat; 
            }
            set
            {
                _flat = value;
            }
        }

        public override string ToString()
        {
            return $"{RelatedPerson.FullName} is {TargetPerson.FullName}'s {Relationship.Type}";
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

        public override string ToString()
        {
            return $"Target: {TargetId}, Related: {RelatedId}, RelId: {RelationshipId}";
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

        public static eGender? Gender(string relIdStr)
        {
            var relId = int.Parse(relIdStr);
            var eRel = Enum.GetValues(typeof(eRel)).Cast<eRel>().First(e => (int)e == relId);
            return Gender(eRel);
        }

        public static eGender? Gender(eRel rel)
        {
            eGender? gender = null;

            switch (rel)
            {
                case eRel.Mother:
                case eRel.Sister:
                case eRel.Daughter:
                case eRel.Wife:
                case eRel.Aunt:
                case eRel.Niece:
                case eRel.MotherInLaw:
                case eRel.SisterInLaw:
                    gender = eGender.Female;
                    break;
                case eRel.Father:
                case eRel.Brother:
                case eRel.Son:
                case eRel.Husband:
                case eRel.Uncle:
                case eRel.Nephew:
                case eRel.FatherInLaw:
                case eRel.BrotherInLaw:
                    gender = eGender.Male;
                    break;
                case eRel.Cousin:
                    break;
            }

            return gender;
        }
    }
}
