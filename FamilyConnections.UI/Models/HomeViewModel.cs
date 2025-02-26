using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;

//using System.Web.Mvc;
//using IConnection = FamilyConnections.Core.Interfaces.IConnection;
using ModelBinderAttribute = Microsoft.AspNetCore.Mvc.ModelBinderAttribute;

namespace FamilyConnections.UI.Models
{
    public class HomeViewModel //: IHomePage
    {
        public HomeViewModel()
        {
                
        }

        public HomeViewModel(List<SelectListItem> personsItems = null, List<PersonViewModel> persons = null, List<ConnectionViewModel> connections = null)
        {
            AllPersonsItems = personsItems ?? new List<SelectListItem>();
            AllPersons = persons ?? new List<PersonViewModel>();
            AllConnections = connections ?? new List<ConnectionViewModel>();
            CurrentPerson = new PersonViewModel();
            CurrentConnection = new ConnectionViewModel();
            Countries = new List<SelectListItem> { new SelectListItem("Israel", "1") };
            Relationships = EnumManager.GetRelationships();
        }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Relationships { get; set; }
        public List<SelectListItem> AllPersonsItems { get; set; }

        public List<PersonViewModel> AllPersons { get; set; }
        public List<ConnectionViewModel> AllConnections { get; set; }

        //[ModelBinder(BinderType = typeof(PersonViewModel))]
        public PersonViewModel CurrentPerson { get; set; }
        public ConnectionViewModel CurrentConnection { get; set; }

        internal ConnectionDTO[] CheckAllConnections()
        {
            var newConnections = new List<ConnectionDTO>();

            foreach (var person in AllPersons)
            {
                // all persons that are not the person in iteration
                var otherPersons = AllPersons.Where(p => p.Id != person.Id);
                // all persons that have first level connection with the person in iteration
                var firstLevel = otherPersons.SelectMany(p => p.Connections.Where(c => c.Key.Id == person.Id).ToList()).ToDictionary(k => k.Key, v => v.Value);
                var secondLevel = firstLevel.SelectMany(p => p.Key.Connections).Distinct();

                foreach (var other in secondLevel)
                {
                    var firstConn = firstLevel.First(p => p.Key.Id == other.Key.Id);

                    if (firstConn.Value == eRel.Husband && other.Value == eRel.Mother)
                    {
                        var newRel = eRel.Uncle;
                        var newConnection = new ConnectionDTO(person.DTO, other.Key.DTO, newRel);
                        newConnections.Add(newConnection);
                    }
                    else if (firstConn.Value == eRel.Husband && other.Value == eRel.Mother)
                    {
                        
                    }
                    else if (firstConn.Value == eRel.Husband && other.Value == eRel.Mother)
                    {

                    }
                    //else if (firstConn.Value == eRel.Husband && other.Value == eRel.Mother)
                    //{
                    //}
                }
            }


            return newConnections.ToArray();
        }

        internal void SetCurrentConnections()
        {
            if (CurrentPerson != null)
            {
                CurrentPerson.Connections = AllConnections.Where(c => c.TargetPerson.Id == CurrentPerson.Id).ToDictionary(k => k.RelatedPerson, v => v.Relationship.Type.Value);
            }
        }
    }
}
