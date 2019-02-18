using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherDTOForAdmin : TeacherDTOForTeacher 
    {
        public TeacherDTOForAdmin()
        {
            Roles = new List<string>();
        }

        [JsonProperty(Order = -3)]
        public IList<string> Roles { get; set; }
        [JsonProperty(Order = 2)]
        public bool EmailConfirmed { get; set; }
        [JsonProperty(Order = 3)]
        public bool PhoneNumberConfirmed { get; set; }




    }
}