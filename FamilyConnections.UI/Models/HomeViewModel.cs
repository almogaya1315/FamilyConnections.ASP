using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
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
            SetConnections();
        }

        public List<SelectListItem> AllPersonsItems { get; set; }

        public List<PersonViewModel> AllPersons { get; set; }
        public List<ConnectionViewModel> AllConnections { get; set; }

        //[ModelBinder(BinderType = typeof(PersonViewModel))]
        public PersonViewModel CurrentPerson { get; set; }

        public void SetConnections()
        {
            var person1 = AllPersons[0];
            var person2 = AllPersons[1];
            var person3 = AllPersons[2];
            var person4 = AllPersons[3];

            AllConnections.Add(new ConnectionViewModel(person1, person2, eRel.Wife));
            AllConnections.Add(new ConnectionViewModel(person1, person3, eRel.Daughter));
            AllConnections.Add(new ConnectionViewModel(person1, person4, eRel.Daughter));

            AllConnections.Add(new ConnectionViewModel(person2, person1, eRel.Husband));
            AllConnections.Add(new ConnectionViewModel(person2, person3, eRel.Daughter));
            AllConnections.Add(new ConnectionViewModel(person2, person4, eRel.Daughter));

            AllConnections.Add(new ConnectionViewModel(person3, person1, eRel.Father));
            AllConnections.Add(new ConnectionViewModel(person3, person2, eRel.Mother));
            AllConnections.Add(new ConnectionViewModel(person3, person4, eRel.Sister));

            AllConnections.Add(new ConnectionViewModel(person4, person1, eRel.Father));
            AllConnections.Add(new ConnectionViewModel(person4, person2, eRel.Mother));
            AllConnections.Add(new ConnectionViewModel(person4, person3, eRel.Sister));
        }

        internal void SetCurrentConnections()
        {
            if (CurrentPerson != null)
            {
                CurrentPerson.Connections = AllConnections.Where(c => c.TargetPerson.Id == CurrentPerson.Id).ToDictionary(k => k.RelatedPerson, v => v.Relationship);
            }
        }
    }
}
