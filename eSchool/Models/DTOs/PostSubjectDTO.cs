using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class PostSubjectDTO
    {
      
        [Required]
        [Display(Name = "Subject name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        //RAZRED
        [Required]
        [Display(Name = "Grade")]
        [Range(1, 8, ErrorMessage = "The {0} must be between {1} and {2}.")]
        public int Grade { get; set; }

        [Required(ErrorMessage = "The {0} attribute must be provided.")] //TODO testiraj
        [Display(Name = "Number of classes per week")]
        public int NumberOfClassesPerWeek { get; set; }
        
    }
}