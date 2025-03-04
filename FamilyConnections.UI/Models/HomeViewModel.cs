using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.Core.Services;
using FamilyConnections.UI.Controllers;
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
                // all persons that are not the person in iteration
                var otherPersons = AllPersons.Where(p => p.Id != person.Id).ToList();

                // all persons that have first level connection with the person in iteration's related person
                var connections = person.SetConnections(AllPersons);
                var connectionToRelated = connections.First();
                var relatedConnections = otherPersons.SelectMany(p => p.SetConnections(otherPersons).Where(c => c.RelatedPerson.Id == connectionToRelated.RelatedPerson.Id).ToList());

                foreach (var relatedConn in relatedConnections)
                {
                    eRel? relatedRel = null;
                    eRel? personRel = null;
                    ConnectionDTO relatedNewConn;
                    ConnectionDTO personNewConn;

                    // if the related's connection's relation is MOTHER, 
                    // and the relation to the related is SISTER or BROTHER,
                    // then the person in iteration's relation to the related's connection in (inner) iteration...
                    if (relatedConn.Relationship.Type == eRel.Mother && IsSibling(connectionToRelated)) 
                    {
                        // is AUNT or UNCLE by gender, for the related
                        relatedRel = person.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;

                        // or NIECE or NEPHEW by gender, for the person
                        personRel = relatedConn.TargetPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                    }
                    else if (relatedConn.Relationship.Type == eRel.Wife && IsSibling(connectionToRelated))
                    {
                        // is SisterInLaw or BrotherInLaw by gender, for the related
                        relatedRel = person.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;

                        // is SisterInLaw or BrotherInLaw by gender, for the person
                        personRel = relatedConn.TargetPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                    }

                    if (relatedRel.HasValue && personRel.HasValue)
                    {
                        relatedNewConn = new ConnectionDTO(relatedConn.TargetPerson.DTO, person.DTO, relatedRel);
                        personNewConn = new ConnectionDTO(person.DTO, relatedConn.TargetPerson.DTO, personRel);
                        newConnections.Add(relatedNewConn);
                        newConnections.Add(personNewConn);
                    }
                    else
                    {
                        _logger.LogWarning($"Unable to find connection between [{person.Id}] and [{relatedConn.TargetPerson.Id}]");
                    }
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
