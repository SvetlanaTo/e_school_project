using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eSchool.Models.DTOs
{
    public class ParentDTOForAdmin : ParentDTOForTeacher 
    {
        public ParentDTOForAdmin() 
        {
            //Roles = new List<IdentityUserRole>();
            Roles = new List<string>();
        }

        [JsonProperty(Order = -3)]
        //public IList<IdentityUserRole> Roles { get; set; }
        public IList<string> Roles { get; set; }
        [JsonProperty(Order = 2)]
        public bool EmailConfirmed { get; set; }
        [JsonProperty(Order = 3)]
        public bool PhoneNumberConfirmed { get; set; }
    }
}