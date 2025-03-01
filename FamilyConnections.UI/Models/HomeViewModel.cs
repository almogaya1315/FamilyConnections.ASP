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

            AllPersons.Reverse();
            foreach (var person in AllPersons)
            {
                // all persons that are not the person in iteration
                var otherPersons = AllPersons.Where(p => p.Id != person.Id).ToList();
                // all persons that have first level connection with the person in iteration's related person
                var related = person.SetConnections(AllPersons).First().RelatedPerson;
                var firstLevelConns = otherPersons.SelectMany(p => p.SetConnections(otherPersons).Where(c => c.RelatedPerson.Id == related.Id).ToList());
                var firstLevel = firstLevelConns.ToDictionary(k => k.TargetPerson, v => v.Relationship.Type);
                //var secondLevel = firstLevel.SelectMany(p => p.Key.SetConnections(firstLevel.Keys.ToList())).Distinct();

                foreach (var other in firstLevelConns)
                {
                    var firstConn = firstLevel.First(p => p.Key.Id == other.TargetPerson.Id);

                    if (firstConn.Value == eRel.Husband && other.Relationship.Type == eRel.Mother)
                    {
                        var newRel = eRel.Uncle;
                        var newConnection = new ConnectionDTO(person.DTO, other.RelatedPerson.DTO, newRel);
                        newConnections.Add(newConnection);
                    }
                    else if (firstConn.Value == eRel.Husband && other.Relationship.Type == eRel.Mother)
                    {
                        
                    }
                    else if (firstConn.Value == eRel.Husband && other.Relationship.Type == eRel.Mother)
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
                CurrentPerson.DTO.FlatConnections = AllConnections.Select(c => c.DTO.Flat).Where(c => c.TargetId == CurrentPerson.Id).ToList();

                //CurrentPerson.Connections = AllConnections.Where(c => c.TargetPerson.Id == CurrentPerson.Id).ToList();
            }
        }
    }
}
