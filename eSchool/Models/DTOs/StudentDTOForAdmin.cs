using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class StudentDTOForAdmin : StudentDTOForTeacher
    {
        public StudentDTOForAdmin() 
        {
            // Roles = new List<IdentityUserRole>(); 
            //Roles = new List<IdentityRole>();
            Roles = new List<string>();
            
        }

        [JsonProperty(Order = -1)]
        //public IList<IdentityUserRole> Roles { get; set; }
        //public IList<IdentityRole> Roles { get; set; }
        public IList<string> Roles { get; set; } 

        //[JsonProperty(Order = 2)]
        public bool EmailConfirmed { get; set; }
       // [JsonProperty(Order = 1)]
        public bool PhoneNumberConfirmed { get; set; }

        
        
    }
}