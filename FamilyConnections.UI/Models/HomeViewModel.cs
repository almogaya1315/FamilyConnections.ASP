using FamilyConnections.BL.Handlers;
using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.Core.Services;
using FamilyConnections.UI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Host.Mef;
using NuGet.Protocol.Plugins;
using System;
using System.Security.Policy;



//using System.Web.Mvc;
//using IConnection = FamilyConnections.Core.Interfaces.IConnection;
using ModelBinderAttribute = Microsoft.AspNetCore.Mvc.ModelBinderAttribute;

namespace FamilyConnections.UI.Models
{
    public class HomeViewModel //: IHomePage
    {
        private readonly ILogger<HomeViewModel> _logger;


        public HomeViewModel()
        {
            _logger = new Logger<HomeViewModel>(new LoggerFactory());
        }

        public HomeViewModel(List<SelectListItem> personsItems = null, List<PersonViewModel> persons = null, List<ConnectionViewModel> connections = null) : this()
        {
            AllPersonsItems = personsItems ?? new List<SelectListItem>();
            AllPersons = persons ?? new List<PersonViewModel>();
            AllConnections = connections ?? new List<ConnectionViewModel>();
            CurrentPerson = new PersonViewModel();
            CurrentConnection = new ConnectionViewModel();
            Countries = new List<SelectListItem> { new SelectListItem("Israel", "1") };
            Relationships = EnumManager.GetEnum<eRel>(toIndex: 15); //GetRelationships();
            Genders = EnumManager.GetEnum<eGender>(); //GetGenders();
        }

        //public List<SelectListItem> RelationshipsBy(eGender gender)
        //{
        //    return Relationships.Where(r => RelationshipInfo.Gender(r.Value) == gender).ToList();
        //}

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Relationships { get; set; }
        public List<SelectListItem> AllPersonsItems { get; set; }
        public List<SelectListItem> Genders { get; set; }

        public List<PersonViewModel> AllPersons { get; set; }
        public List<ConnectionViewModel> AllConnections { get; set; }

        //[ModelBinder(BinderType = typeof(PersonViewModel))]
        public PersonViewModel CurrentPerson { get; set; }
        public ConnectionViewModel CurrentConnection { get; set; }

        internal List<ConnectionDTO> CheckAllConnections()
        {
            var newConnections = new List<ConnectionDTO>();

            var possibleComplex_debug = new List<ConnectionDTO>();
            var FarRelConnections_debug = new List<(ConnectionDTO Target, ConnectionDTO Related)>();

            // reverse to start with the new added person
            AllPersons.Reverse();
            foreach (var person in AllPersons)
            {
                try
                {
                    var personConnections = person.GetConnections(AllPersons);
                    var relatedConnections = personConnections.SelectMany(c => c.RelatedPerson.GetConnections(AllPersons)).ToList();
                    relatedConnections.RemoveAll(rc => rc.RelatedPerson.Id == person.Id);
                    foreach (var relatedConnection in relatedConnections)
                    {
                        eRel? relation = null;
                        var personConnection = personConnections.Find(pc => pc.RelatedPerson.Id == relatedConnection.TargetPerson.Id);

                        //ComplexRel -> Step, InLaw, Great, Ex, Far
                        eRel? possibleComplexRel = null;

                        ConnectionsHandler.InitConnection(personConnection.DTO, relatedConnection.DTO, AllConnections.Select(c => c.DTO));
                        relation = ConnectionsHandler.FindRelation(out possibleComplexRel);

                        //if (relation == eRel.FarRel) FarRelConnections_debug.Add((personConnection.DTO, relatedConnection.DTO));

                        ConnectionsHandler.ConnectionBetween(person.DTO, relatedConnection.RelatedPerson.DTO, relation.Value, 
                            ref newConnections, possibleComplexRel, ref possibleComplex_debug);
                        
                        //ConnectionsHandler.ConnectionBetween(relatedConnection.RelatedPerson.DTO, person.DTO, relation.Value, 
                        //    ref newConnections, possibleComplexRel, ref possibleComplex_debug, opposite: true);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error in 'HomeViewModel.CheckAllConnections' - target id [{person.Id}]");
                }
            }

            return newConnections;
        }

        //private bool ConnExists(PersonViewModel person, PersonViewModel related, eRel relation, ref List<ConnectionDTO> newConnections)
        //{
        //    var existsInNew = newConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
        //    var existsInAll = AllConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
        //    return existsInNew || existsInAll;
        //}

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
