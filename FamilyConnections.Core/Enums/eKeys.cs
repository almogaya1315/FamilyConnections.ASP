using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Enums
{
    public enum eKeys
    {
        currentPerson,
        allPersons,
        flatConnections
    }

    public enum eModelStateKeys
    {
        TargetPerson_FullName,
        TargetPerson_DateOfBirth,
        TargetPerson_PlaceOfBirth,
        RelatedPerson_Id,
        Relationship_Id
    }
}
