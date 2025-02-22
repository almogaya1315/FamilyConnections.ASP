using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;

namespace FamilyConnections.UI.Models
{
    public class HomeViewModel
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
        }

        public List<SelectListItem> AllPersonsItems { get; set; }
        public List<PersonViewModel> AllPersons { get; set; }
        public List<ConnectionViewModel> AllConnections { get; set; }

        //public SelectListItem CurrentPersonItem
        //{
        //    get
        //    {
        //        var personItem = new SelectListItem("", "-1");
        //        if (CurrentPerson.Id > -1)
        //            personItem = AllPersons.Find(p => p.Value == CurrentPerson.Id.ToString());
        //        return personItem;
        //    }
        //    set
        //    {
        //        CurrentPersonItem = value;
        //    }
        //}
        public PersonViewModel CurrentPerson { get; set; }

        internal void SetCurrentConnections()
        {
            if (CurrentPerson != null)
            {
                CurrentPerson.Connections = AllConnections.Where(c => c.TargetPerson.Id == CurrentPerson.Id).ToDictionary(k => k.RelatedPerson, v => v.Relationship);
            }
        }
    }

    public class ConnectionViewModel
    {
        public ConnectionViewModel(PersonViewModel target, PersonViewModel related, eRel rel)
        {
            TargetPerson = target;
            RelatedPerson = related;
            Relationship = rel;
        }

        public PersonViewModel TargetPerson { get; set; }
        public PersonViewModel RelatedPerson { get; set; }
        public eRel Relationship { get; set; }
    }

    public class PersonViewModel
    {
        public PersonViewModel()
        {
            Id = -1;
            Connections = new Dictionary<PersonViewModel, eRel>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }

        public DateTime DateOfBirth;
        public string DateOfBirthStr
        {
            get
            {
                return DateOfBirth.ToString("dd/MM/yyyy");
            }
            set
            {
                DateOfBirth = DateTime.Parse(value);
            }
        }
        public string PlaceOfBirth { get; set; }
        public Dictionary<PersonViewModel, eRel> Connections { get; set; }
    }

    public enum eRel
    {
        Mother,
        Father,
        Sister,
        Brother,
        Daughter,
        Son,
        Wife,
        Husband,
        Aunt,
        Uncle,
        Cousin,
        Niece,
        Nephew,
    }
}
