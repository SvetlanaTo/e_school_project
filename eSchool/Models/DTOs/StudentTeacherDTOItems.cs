using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class StudentTeacherDTOItems
    {
        public StudentTeacherDTOItems()
        {
            Teachers = new List<TeacherDTOItem>();
        }
        
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Student { get; set; } //ime i prezime

        public string Form { get; set; } //odeljenje: grade - tag

        public int NumberOfTeachers { get; set; } 

        public IList<TeacherDTOItem> Teachers { get; set; }
    }
}