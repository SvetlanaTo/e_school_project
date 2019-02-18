using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class RegisterStudentDTO
    {
        [Required]
        [Display(Name = "User name")]
        [StringLength(50, ErrorMessage = "The {0} must be in between {2} and {50} characters long.", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
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


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{5,15})",
            ErrorMessage = "{0} must be between 5 and 15 character in length, must contains at least one digit from 0-9 and one lowercase characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and {0} do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "JMBG")]
        [StringLength(13, ErrorMessage = "The {0} must be 13 characters long.", MinimumLength = 13)]
        public string Jmbg { get; set; }

        [Required(ErrorMessage = "The {0} attribute must be provided.")]
        [Display(Name = "Day of birth")]
        public DateTime DayOfBirth { get; set; }
      
        [Required]
        [Display(Name = "Parent User name")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string ParentUserName { get; set; }

        [Required]
        [Display(Name = "Parent First name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string ParentFirstName { get; set; }

        [Required]
        [Display(Name = "Parent Last name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string ParentLastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress] //validacija
        [Required(ErrorMessage = "{0} is Required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect Email Format")] 
        public string ParentEmail { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{5,15})",
            ErrorMessage = "{0} must be between 5 and 15 character in length, must contains at least one digit from 0-9 and one lowercase characters")]
        public string ParentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and {0} do not match.")]
        public string ParentConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Parent JMBG")]
        [StringLength(13, ErrorMessage = "The {0} must be 13 characters long.", MinimumLength = 13)]
        public string ParentJmbg { get; set; } 

        [Required(ErrorMessage = "The {0} attribute must be provided.")]
        [Display(Name = "Parent Mobile phone")]
        public string ParentMobilePhone { get; set; }

        //jedan ucenik pripada tacno jednom odeljenju
        [Required(ErrorMessage = "The {0} attribute must be provided.")]
        [Display(Name = "Form ID")]
        public int FormId { get; set; }
    }
}

