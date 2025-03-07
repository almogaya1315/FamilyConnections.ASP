using FamilyConnections.BL.Handlers;
using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.Core.Services;
using FamilyConnections.UI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            Relationships = EnumManager.GetEnum<eRel>(); //GetRelationships();
            Genders = EnumManager.GetEnum<eGender>(); //GetGenders();
        }

        public List<SelectListItem> RelationshipsBy(eGender gender)
        {
            return Relationships.Where(r => RelationshipInfo.Gender(r.Value) == gender).ToList();
        }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Relationships { get; set; }
        public List<SelectListItem> AllPersonsItems { get; set; }
        public List<SelectListItem> Genders { get; set; }

        public List<PersonViewModel> AllPersons { get; set; }
        public List<ConnectionViewModel> AllConnections { get; set; }

        //[ModelBinder(BinderType = typeof(PersonViewModel))]
        public PersonViewModel CurrentPerson { get; set; }
        public ConnectionViewModel CurrentConnection { get; set; }

        internal ConnectionDTO[] CheckAllConnections()
        {
            var newConnections = new List<ConnectionDTO>();

            // reverse to start with the new added person
            AllPersons.Reverse();
            foreach (var person in AllPersons)
            {
                try
                {
                    var personConnections = person.GetConnections(AllPersons); //SetConnections(AllPersons);
                    var relatedConnections = personConnections.SelectMany(c => c.RelatedPerson.GetConnections(AllPersons)).ToList();
                    foreach (var relatedConnection in relatedConnections)
                    {
                        eRel? relation = null;
                        var personConnection = personConnections.Find(pc => pc.RelatedPerson.Id == relatedConnection.TargetPerson.Id);

                        //if (relatedConnection.RelatedPerson.Id == personConnection.RelatedPerson.Id &&
                        //    relatedConnection.Relationship.Type == personConnection.Relationship.Type)
                        //{
                        //    _logger.LogInformation($"Connection between target id [{person.Id}] and related id [{relatedConnection.Id}] allready exists.");
                        //    continue;
                        //}

                        // person's Sister or Brother
                        if (IsSibling(personConnection))
                        {
                            // has Husband or Wife
                            if (IsSpouse(relatedConnection))
                            {
                                // person will have SisterInLaw or BrotherInLaw
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                            }
                            // has Daughter or Son
                            else if (IsChild(relatedConnection))
                            {
                                // person will have Niece or Nephew
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                            }
                            // has Mother or Father
                            else if (IsParent(relatedConnection))
                            {
                                // person will have Mother or Father
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                            }
                        }
                        // person's Husband or Wife
                        else if (IsSpouse(personConnection))
                        {

                        }
                        else
                        {

                        }



                        if (relation.HasValue)
                        {
                            if (!ConnExists(person, relatedConnection.TargetPerson, relation.Value, ref newConnections))
                            {
                                var newConn = new ConnectionDTO(person.DTO, relatedConnection.RelatedPerson.DTO, relation);
                                newConnections.Add(newConn);
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Unable to find connection between target id [{person.Id}] and related id [{relatedConnection.TargetPerson.Id}]");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error in 'HomeViewModel.CheckAllConnections' - target id [{person.Id}]");
                }
            }

            return newConnections.ToArray();
        }

        private bool ConnExists(PersonViewModel person, PersonViewModel related, eRel relation, ref List<ConnectionDTO> newConnections)
        {
            var existsInNew = newConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            var existsInAll = AllConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            return existsInNew || existsInAll;
        }

        private bool IsSpouse(ConnectionViewModel relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Wife || relatedConnection.Relationship.Type == eRel.Husband;
        }

        private bool IsChild(ConnectionViewModel relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Daughter || relatedConnection.Relationship.Type == eRel.Son;
        }

        private bool IsParent(ConnectionViewModel relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Mother || relatedConnection.Relationship.Type == eRel.Father;
        }

        private bool IsSibling(ConnectionViewModel relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Sister || relatedConnection.Relationship.Type == eRel.Brother;
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
