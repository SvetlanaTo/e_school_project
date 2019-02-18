using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class MarkValuesListDTO  
    {
        public MarkValuesListDTO()
        {
            FirstSemesterMarks = new List<int>();
            SecondSemesterMarks = new List<int>();
        }

       
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string TeacherID { get; set; }
        public string Teacher { get; set; } 
        public string ParentID { get; set; } 
        public string StudentID { get; set; }
        public string Student { get; set; }

        public IList<int> FirstSemesterMarks { get; set; }
        public IList<int> SecondSemesterMarks { get; set; } 
        
         
    } 
}