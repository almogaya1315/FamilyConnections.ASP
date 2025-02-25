using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public ConnectionViewModel(ConnectionDTO dto)
        {
            _connection = dto;

            //TargetPerson = new PersonViewModel(dto.TargetPerson);
            //RelatedPerson = new PersonViewModel(dto.RelatedPerson);
            //Relationship = dto.Relationship;
        }

        public ConnectionViewModel(PersonViewModel target, PersonViewModel related, eRel? rel)
        {
            _connection.TargetPerson = target.DTO;
            _connection.RelatedPerson = related.DTO;
            _connection.Relationship = new RelationshipInfo(rel);

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
    }
}
