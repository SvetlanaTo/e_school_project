using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class ChangePassDTO
    {
        [JsonProperty("ID")]
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{5,15})",
            ErrorMessage = "{0} must be between 5 and 15 character in length, must contains at least one digit from 0-9 and one lowercase characters")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{5,15})",
            ErrorMessage = "{0} must be between 5 and 15 character in length, must contains at least one digit from 0-9 and one lowercase characters")]
        public string NewPassword { get; set; }

    }
}