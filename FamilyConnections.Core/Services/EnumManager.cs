using FamilyConnections.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Services
{
    public class EnumManager
    {
        public static List<SelectListItem> GetRelationships()
        {
            return Enum.GetValues(typeof(eRel)).Cast<eRel>().Select(e => new SelectListItem(e.ToString(), ((int)e).ToString())).ToList();
        }
    }
}
