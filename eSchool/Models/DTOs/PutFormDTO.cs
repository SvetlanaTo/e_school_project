using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{ 
    public class PutFormDTO
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [Display(Name = "Grade")]
        [Range(1, 8, ErrorMessage = "The {0} must be within the range of {1} and {2}.")]
        public int? Grade { get; set; }

        [Display(Name = "Tag")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Tag { get; set; }

        [Display(Name = "Grade started")]
        public DateTime? Started { get; set; }

        [Display(Name = "Attending teacher ID")]
        public string AttendingTeacherId { get; set; } 
    }
}