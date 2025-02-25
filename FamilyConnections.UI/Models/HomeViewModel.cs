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

        internal List<ConnectionViewModel> CheckAllConnections()
        {
            return null;
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
