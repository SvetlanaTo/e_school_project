using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class StudentDTOForList
    {
        [JsonProperty("ID", Order = -1)]
        public string Id { get; set; } 

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Jmbg { get; set; }

        public DateTime DayOfBirth { get; set; }

        public string ParentID { get; set; }
        public int FormID { get; set; }




    }
}