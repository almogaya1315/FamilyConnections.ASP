using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.Core.Services;
using FamilyConnections.UI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
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
                    // all persons that are not the person in iteration
                    var otherPersons = AllPersons.Where(p => p.Id != person.Id).ToList();

                    // all persons that have first level connection with the person in iteration's related person
                    var connections = person.SetConnections(AllPersons);
                    var othersConnections = otherPersons.SelectMany(p => p.SetConnections(AllPersons)).ToList();
                    var relatedConnections = othersConnections.Select(c => (Conn: c, connectionToRelated: connections.Find(r => c.RelatedPerson.Id == r.RelatedPerson.Id)))
                                                              .Where(rc => rc.connectionToRelated != null).ToList();

                    foreach (var related in relatedConnections)
                    {
                        eRel? relation = null;

                        // if the related's connection's relation is MOTHER or WIFE, 
                        // and the relation TO the related is SISTER or BROTHER,
                        // then the person in iteration's relation to the related's connection in (inner) iteration...
                        if (related.Conn.Relationship.Type == eRel.Mother && IsSibling(related.connectionToRelated))
                        {
                            // is NIECE or NEPHEW by gender, for the person
                            relation = related.Conn.TargetPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                        }
                        else if (related.Conn.Relationship.Type == eRel.Wife && IsSibling(related.connectionToRelated))
                        {
                            // is SisterInLaw or BrotherInLaw by gender, for the person
                            relation = related.Conn.TargetPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                        }
                        // if the related's connection's relation is SISTER, 
                        // and the relation OF the related is SISTER or BROTHER,
                        // then the person in iteration's relation to the related's connection in (inner) iteration...
                        else if (related.Conn.Relationship.Type == eRel.Sister && IsSibling(related.Conn))
                        {
                            // is AUNT or UNCLE by gender, for the person
                            relation = related.Conn.TargetPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                        }



                        if (relation.HasValue)
                        {
                            var newConn = new ConnectionDTO(person.DTO, related.Conn.TargetPerson.DTO, relation);
                            newConnections.Add(newConn);
                        }
                        else
                        {
                            _logger.LogWarning($"Unable to find connection between target id [{person.Id}] and related id [{related.Conn.TargetPerson.Id}]");
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

        private bool IsSibling(ConnectionViewModel connectionToRelated)
        {
            return connectionToRelated.Relationship.Type == eRel.Sister || connectionToRelated.Relationship.Type == eRel.Brother;
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
