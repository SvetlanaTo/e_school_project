using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class PutFormToTeacherSubjectDTO
    {
        [JsonProperty("ID")] 
        public int Id { get; set; }

        [Required]
        public int FormId { get; set; }
        [Required]
        public string TeacherId { get; set; }
        [Required]
        public int SubjectId { get; set; }



    }
}