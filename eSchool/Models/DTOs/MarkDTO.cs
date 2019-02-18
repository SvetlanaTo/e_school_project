using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class MarkDTO
    {
        [JsonProperty("ID", Order = -1)]
        public int Id { get; set; }

        public int MarkValue { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Semesters Semester { get; set; }
        public DateTime Created { get; set; }

        public string StudentID { get; set; }
        public string Student { get; set; }     

        public int SubjectID { get; set; }
        public string SubjectName { get; set; }

        public string TeacherID { get; set; }
        public string Teacher { get; set; } 

        public string ParentID { get; set; }
        public int FormID { get; set; }
        public string AttendingTeacherID { get; set; }

        public int TeacherToSubjectID { get; set; }
        public int FormToTeacherToSubjectID { get; set; }



    }
}