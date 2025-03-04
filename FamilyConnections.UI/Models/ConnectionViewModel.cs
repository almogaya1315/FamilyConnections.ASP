using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FamilyConnections.UI.Models
{
    public class ConnectionViewModel //: IConnection
    {
        public ConnectionDTO _connection { get; set; }

        public ConnectionDTO DTO
        {
            get
            {
                return _connection;
            }
        }

        public ConnectionViewModel() 
        {
            _connection = new ConnectionDTO();
        }

        //public ConnectionViewModel(FlatConnection flat) : this()
        //{
        //    _connection.Flat = flat;
        //}

        public ConnectionViewModel(ConnectionDTO dto)
        {
            _connection = dto;

            //TargetPerson = new PersonViewModel(dto.TargetPerson);
            //RelatedPerson = new PersonViewModel(dto.RelatedPerson);
            //Relationship = dto.Relationship;
        }

        public ConnectionViewModel(PersonViewModel target, PersonViewModel related, eRel? rel)
        {
            _connection = new ConnectionDTO(target.DTO, related.DTO, rel);

            //TargetPerson = target;
            //RelatedPerson = related;
            //Relationship = new RelationshipInfo(rel);
        }

        public PersonViewModel TargetPerson 
        {
            get 
            {
                return new PersonViewModel(_connection.TargetPerson);
            }
            set 
            {
                _connection.TargetPerson = value.DTO;
            }
        }
        public PersonViewModel RelatedPerson 
        {
            get 
            {
                return new PersonViewModel(_connection.RelatedPerson);
            }
            set 
            {
                _connection.RelatedPerson = value.DTO;
            }
        }
        public RelationshipInfo Relationship 
        {
            get 
            {
                return _connection.Relationship;
            }
            set 
            {
                _connection.Relationship = value;
            } 
        }

        //internal KeyValuePair<PersonViewModel, eRel> ToKeyValuePair()
        //{
        //    return new KeyValuePair<PersonViewModel, eRel>(TargetPerson, Relationship.Type.Value);
        //}

        public override string ToString()
        {
            return $"Target: {TargetPerson.FullName}, Related: {RelatedPerson.FullName}, Rel: {Relationship.Type}";
        }
    }
}
