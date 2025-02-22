using FamilyConnections.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Interfaces
{
    public interface IConnection
    {
        public IPerson TargetPerson { get; set; }
        public IPerson RelatedPerson { get; set; }
        public eRel Relationship { get; set; }
    }
}
