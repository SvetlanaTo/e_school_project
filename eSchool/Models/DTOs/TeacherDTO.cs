using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherDTO
    {       
        [JsonProperty("ID")]
        public string Id { get; set; } 

        public string UserName { get; set; }
 
        public string FirstName { get; set; }
 
        public string LastName { get; set; }

        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Genders? Gender { get; set; }
    }
}