using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherIDWeeklyEngagementsDTO 
    {
        public string TeacherID { get; set; }
        public string Teacher { get; set; }
        [Required] 
        public int? WeeklyEngagements { get; set; }    
    }
}