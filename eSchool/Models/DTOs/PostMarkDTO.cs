using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class PostMarkDTO
    {

        [Required]
        [Display(Name = "Mark value")]
        [Range(1, 5, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int MarkValue { get; set; }

    }
}