using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class PutParentDTO
    {
        [JsonProperty("ID")]
        public string Id { get; set; }

        [Display(Name = "User name")]
        [StringLength(50, ErrorMessage = "The {0} must be in between {2} and {50} characters long.", MinimumLength = 4)]
        public string UserName { get; set; }

        [Display(Name = "First name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress] //validacija
        [Required(ErrorMessage = "{0} is Required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect Email Format")]//TODO validation
        public string Email { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool? EmailConfirmed { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Phone Number Confirmed")]
        public bool? PhoneNumberConfirmed { get; set; }

        [Display(Name = "JMBG")]
        [StringLength(13, ErrorMessage = "The {0} must be 13 characters long.", MinimumLength = 13)]
        public string Jmbg { get; set; }

        [Display(Name = "Mobile phone")] 
        public string MobilePhone { get; set; }
    }
}