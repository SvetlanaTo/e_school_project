using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class StudentDTOForStudentAndParent
    {
        [JsonProperty("ID", Order = -3)] 
        public string Id { get; set; }
        [JsonProperty(Order = -2)]
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public virtual ParentDTOForStudentAndParents Parent { get; set; }
        public virtual FormDTOForStudentAndParents Form { get; set; }
    }
}