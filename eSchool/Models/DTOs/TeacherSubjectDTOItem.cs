using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherSubjectDTOItem
    {
        public string TeacherId { get; set; }
        public string Teacher { get; set; }

        public int SubjectId { get; set; }
        public string Subject { get; set; } 

    }
}