using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;

namespace FamilyConnections.UI.Models
{
    public class ConnectionViewModel : IConnection
    {
        public ConnectionViewModel(IPerson target, IPerson related, eRel rel)
        {
            TargetPerson = target;
            RelatedPerson = related;
            Relationship = rel;
        }

        public IPerson TargetPerson { get; set; }
        public IPerson RelatedPerson { get; set; }
        public eRel Relationship { get; set; }
    }
}
