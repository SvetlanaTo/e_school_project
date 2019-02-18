using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class SubjectTeacherSubjectDTOItems
    {
        public SubjectTeacherSubjectDTOItems()
        {
            Teachers = new List<TeacherSubjectDTOItemForSubject>();
        }

        public int SubjectId { get; set; } 

        public string Name { get; set; }

        public int Grade { get; set; }  

        public int NumberOfClassesPerWeek { get; set; } 

        public IList<TeacherSubjectDTOItemForSubject> Teachers { get; set; } 
    }
}