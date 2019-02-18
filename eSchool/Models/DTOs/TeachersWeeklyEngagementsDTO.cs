using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeachersWeeklyEngagementsDTO
    {
        public TeachersWeeklyEngagementsDTO()
        {
            WeeklyEngagementsByTeachers = new List<TeacherIDWeeklyEngagementsDTO>(); 
        }

        public DateTime OnDate { get; set; }
        public IList<TeacherIDWeeklyEngagementsDTO> WeeklyEngagementsByTeachers { get; set; }  
    }
}