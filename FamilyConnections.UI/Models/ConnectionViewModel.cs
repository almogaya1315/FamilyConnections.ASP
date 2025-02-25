using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;

namespace FamilyConnections.UI.Models
{
    public class ConnectionViewModel //: IConnection
    {
        public ConnectionViewModel()
        {

        }

        public ConnectionViewModel(PersonViewModel target, PersonViewModel related, eRel rel)
        {
            TargetPerson = target;
            RelatedPerson = related;
            Relationship = rel;
        }

        public PersonViewModel TargetPerson { get; set; }
        public PersonViewModel RelatedPerson { get; set; }
        public eRel? Relationship { get; set; }
    }
}
