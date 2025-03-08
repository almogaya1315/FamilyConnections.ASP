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
        //public static List<SelectListItem> GetRelationships()
        //{
        //    return Enum.GetValues(typeof(eRel)).Cast<eRel>().Select(e => new SelectListItem(e.ToString(), ((int)e).ToString())).ToList();
        //}
        //public static List<SelectListItem> GetGenders()
        //{
        //    return Enum.GetValues(typeof(eGender)).Cast<eGender>().Select(e => new SelectListItem(e.ToString(), ((int)e).ToString())).ToList();
        //}
        public static List<SelectListItem> GetEnum<T>(int? toIndex = null) where T : Enum
        {
            var @enums = Enum.GetValues(typeof(T)).Cast<T>().Select(e => new SelectListItem(e.ToString(), Convert.ToInt32(e).ToString())).ToList();
            if (toIndex.HasValue) @enums.RemoveAll(e => int.Parse(e.Value) > toIndex.Value);
            return @enums;
        }
    }
}
