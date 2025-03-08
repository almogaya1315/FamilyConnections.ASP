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

            var possibleComplex_debug = new List<ConnectionDTO>();
            var unknownsConnections_debug = new List<(ConnectionDTO Target, ConnectionDTO Related)>();

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

                        //ComplexRel -> Step,InLaw,Great,Ex
                        eRel? possibleComplexRel = null;

                        //if (relatedConnection.RelatedPerson.Id == personConnection.RelatedPerson.Id &&
                        //    relatedConnection.Relationship.Type == personConnection.Relationship.Type)
                        //{
                        //    _logger.LogInformation($"Connection between target id [{person.Id}] and related id [{relatedConnection.Id}] allready exists.");
                        //    continue;
                        //}

                        //person.DTO.Init(personConnection.DTO, relatedConnection.DTO);
                        //relation = person.DTO.HasSibling().That().HasSpouse().Then().IsSiblingInLaw()
                        //                                  .Or().HasChild().Then().IsSiblingChild()
                        //                                  .Or().HasParent().Then().IsParent()
                        //                                  .Or().HasSibling().Then().IsSibling()
                        //                 .Or().HasSpouse().That().HasXXX().Then().IsXXX() 
                        //                                  .Or().HasXXX().Then().IsXXX()
                        //                                  .GetRel();

                        //person's Parent(Mother, Father)
                        if (HasParent(personConnection.DTO))
                        {
                            //HasParent
                            if (HasParent(relatedConnection.DTO))
                            {
                                //GrandParent(GrandMother, GarndFather)
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.GrandMother : eRel.GrandFather;
                            }
                            //HasChild
                            else if (HasChild(relatedConnection.DTO))
                            {
                                //Sibling(Sister, Brother)
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
                            }
                            //HasSibling
                            else if (HasSibling(relatedConnection.DTO))
                            {
                                //ParentSibling(Aunt, Uncle)
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                            }
                            //HasSpouse
                            else if (HasSpouse(relatedConnection.DTO))
                            {
                                //Parent
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                                //StepParent
                                possibleComplexRel = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
                            }
                        }
                        //person's Child(Daughter, Son)
                        else if (HasChild(personConnection.DTO))
                        {
                            //HasParent
                            if (HasParent(relatedConnection.DTO))
                            {
                                //Spouse(Wife, Husband)
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Wife : eRel.Husband;
                                //ExPartner
                                possibleComplexRel = eRel.ExPartner;
                            }
                            //HasChild
                            else if (HasChild(relatedConnection.DTO))
                            {
                                //GrandChild
                                relation = eRel.GrandChild;
                            }
                            //HasSibling
                            else if (HasSibling(relatedConnection.DTO))
                            {
                                //Child
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Daughter : eRel.Son;
                                //StepChild
                                possibleComplexRel = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
                            }
                            //HasSpouse
                            else if (HasSpouse(relatedConnection.DTO))
                            {
                                //ChildInLaw
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.DaughterInLaw : eRel.SonInLaw;
                            }
                        }
                        //person's Sibling
                        else if (HasSibling(personConnection.DTO))
                        {
                            //HasParent
                            if (HasParent(relatedConnection.DTO))
                            {
                                //Parent
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                                //StepParent
                                possibleComplexRel = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
                            }
                            //HasChild
                            else if (HasChild(relatedConnection.DTO))
                            {
                                //SiblingChild(Niece, Nephew)
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                            }
                            //HasSibling
                            else if (HasSibling(relatedConnection.DTO))
                            {
                                //Sibling
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
                                //StepSibling
                                possibleComplexRel = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                            }
                            //HasSpouse
                            else if (HasSpouse(relatedConnection.DTO))
                            {
                                //SiblingInLaw
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                            }
                        }
                        //person's Spouse
                        else if (HasSpouse(personConnection.DTO))
                        {
                            //HasParent
                            if (HasParent(relatedConnection.DTO))
                            {
                                //ParentInLaw
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.MotherInLaw : eRel.FatherInLaw;
                            }
                            //HasChild
                            else if (HasChild(relatedConnection.DTO))
                            {
                                //Child
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Daughter : eRel.Son;
                                //StepChild
                                possibleComplexRel = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
                            }
                            //HasSibling
                            else if (HasSibling(relatedConnection.DTO))
                            {
                                //SiblingInLaw
                                relation = relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                            }
                        }
                        else
                        {
                            unknownsConnections_debug.Add((personConnection.DTO, relatedConnection.DTO));
                        }


                        if (relation.HasValue)
                        {
                            if (!ConnExists(person, relatedConnection.RelatedPerson, relation.Value, ref newConnections))
                            {
                                var newConn = new ConnectionDTO(person.DTO, relatedConnection.RelatedPerson.DTO, relation, possibleComplexRel);
                                if (possibleComplexRel.HasValue) possibleComplex_debug.Add(newConn);
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

        private bool HasSpouse(ConnectionDTO relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Wife || relatedConnection.Relationship.Type == eRel.Husband;
        }

        private bool HasChild(ConnectionDTO relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Daughter || relatedConnection.Relationship.Type == eRel.Son;
        }

        private bool HasParent(ConnectionDTO relatedConnection)
        {
            return relatedConnection.Relationship.Type == eRel.Mother || relatedConnection.Relationship.Type == eRel.Father;
        }

        private bool HasSibling(ConnectionDTO relatedConnection)
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
