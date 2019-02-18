using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class StudentDTOForTeacher : StudentDTOForStudentAndParent 
    {
        public string Jmbg { get; set; }

        public DateTime DayOfBirth { get; set; }

        public string ImagePath { get; set; }

        public bool IsActive { get; set; }

       
    }
}