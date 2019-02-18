using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherSubjectDTOItemForSubject
    {
        public string TeacherId { get; set; }
        public string Teacher { get; set; }
        public DateTime StartedTeaching { get; set; }
        public DateTime? StoppedTeaching { get; set; }  
    }
}