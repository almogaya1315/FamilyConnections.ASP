using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.UI.Converters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json.Serialization;

namespace FamilyConnections.UI.Models
{
    public class PersonViewModel //: IPerson //, IModelBinder
    {
        private PersonDTO _person;

        public PersonDTO DTO
        {
            get
            {
                return _person;
            }
        }

        //public PersonViewModel(PersonViewModel vm)
        //{
        //    _person = new PersonDTO
        //    {
        //        FullName = vm.FullName,
        //        DateOfBirth = vm.DateOfBirth,
        //        PlaceOfBirth = vm.PlaceOfBirth,
        //    };
        //}
        public PersonViewModel(PersonDTO personDTO)
        {
            _person = personDTO;
        }

        public PersonViewModel()
        {
            _person = new PersonDTO();
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

        public DateTime? DateOfBirth
        {
            get
            {
                return _person.DateOfBirth;
            }
            set
            {
                _person.DateOfBirth = value.GetValueOrDefault();
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
        public eGender? Gender 
        {
            get
            {
                return _person.Gender;
            }
            set
            {
                _person.Gender = value;
            } 
        }

        public string Age
        {
            get
            {
                return _person.Age.ToString();
            }
        }

        //[JsonConverter(typeof(PersonViewModelDictionaryConverter))]
        //public Dictionary<PersonViewModel, eRel> Connections 
        //{
        //    get
        //    {
        //        return _person.Connections.ToDictionary(k => new PersonViewModel(k.Key), v => v.Value);
        //    }
        //    set
        //    {
        //        _person.Connections = value.ToDictionary(k => k.Key.DTO, v => v.Value);
        //    }
        //}

        public List<ConnectionViewModel> SetConnections(List<PersonViewModel> persons, ConnectionViewModel newConnection = null)
        {
            var personsScope = persons.ToList();
            if (!personsScope.Exists(p => p.Id == _person.Id)) personsScope.Add(this);

            if (newConnection != null)
            {
                newConnection.DTO.Flat = new FlatConnection(newConnection.TargetPerson.Id, newConnection.RelatedPerson.Id, newConnection.Relationship.Id);
                _person.FlatConnections.Add(newConnection.DTO.Flat);
            }

            return Connections = _person.FlatConnections.Select(f => new ConnectionViewModel(
                                                                     personsScope.Find(p => p.Id == f.TargetId),
                                                                     personsScope.Find(p => p.Id == f.RelatedId),
                                                                     RelationshipInfo.Get(f.RelationshipId))).ToList();
        }

        public List<ConnectionViewModel> Connections 
        { 
            get; 
            set; 
        }

        //public Task BindModelAsync(ModelBindingContext bindingContext)
        //{
        //    var model = new PersonViewModel();
        //    bindingContext.Result = ModelBindingResult.Success(model);
        //    return Task.CompletedTask;
        //}
    }
}
