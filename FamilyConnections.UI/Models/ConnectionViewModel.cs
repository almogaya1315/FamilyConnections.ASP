using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyConnections.UI.Models
{
    public class ConnectionViewModel
    {
        public ConnectionViewModel()
        {
            Connecions = new Dictionary<ConnectionViewModel, eRel>();
            AllUsers = new List<SelectListItem>();
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
        public Dictionary<ConnectionViewModel, eRel> Connecions { get; set; }
        public List<SelectListItem> AllUsers { get; set; }
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
