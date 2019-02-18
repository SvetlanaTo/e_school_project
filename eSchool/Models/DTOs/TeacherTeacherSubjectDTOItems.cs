using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherTeacherSubjectDTOItems
    {
        public TeacherTeacherSubjectDTOItems()
        {
            Subjects = new List<TeacherSubjectDTOItemForTeacher>();
        }

        public string TeacherId { get; set; }

        public string Teacher { get; set; }

        public IList<TeacherSubjectDTOItemForTeacher> Subjects { get; set; } 
    }
}