using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class PostFormDTO
    {
        [Required]
        [Display(Name = "Grade")]
        [Range(1, 8, ErrorMessage = "The {0} must be within the range of {1} and {2}.")]
        public int Grade { get; set; }

        //OZNAKA ODELJENJA
        [Required]
        [Display(Name = "Tag")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Tag { get; set; }

        //Class of       
        [Display(Name = "Grade started")]
        public DateTime Started { get; set; } = new DateTime(2018, 9, 1); //svake skolske godine se ovde promeni godina

        //RAZREDNI STARESINA
        //odeljenje ima tacno jednog nastavnika koji je razredni staresina
        [Required(ErrorMessage = "The {0} attribute must be provided.")] 
        [Display(Name = "Attending Teacher Id:guid")]
        public string AttendingTeacherId { get; set; } 
    }
}