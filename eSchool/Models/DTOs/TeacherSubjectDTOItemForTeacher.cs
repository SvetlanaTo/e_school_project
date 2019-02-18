using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherSubjectDTOItemForTeacher 
    {
        public int SubjectId { get; set; }
        public string Subject { get; set; } 
        public DateTime StartedTeaching { get; set; }
        public DateTime? StoppedTeaching { get; set; }
    }
}