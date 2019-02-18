using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    //ne dozvoljavamo menjanja bilo kog drugog obelezja
    //student, roditelj, predmet,... za njihovu promenu, potrebno je ocenu obrisati i kreirati novu
    public class PutMarkDTO 
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [Display(Name = "Mark value")]
        [Range(1, 5, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int? MarkValue { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Semesters? Semester { get; set; }

        //public DateTime Created { get; set; } //sa UPDATE  Created = DateTime.UtcNow


    }
}