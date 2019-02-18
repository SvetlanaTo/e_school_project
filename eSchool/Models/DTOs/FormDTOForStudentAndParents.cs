using Newtonsoft.Json;
using System;

namespace eSchool.Models.DTOs
{
    public class FormDTOForStudentAndParents
    {
        [JsonProperty("ID", Order = -1)]
        public int Id { get; set; }

        public int Grade { get; set; }

        public string Tag { get; set; }

        public DateTime Started { get; set; } 

        //RAZREDNI STARESINA
        public virtual TeacherDTOForStudentAndParent AttendingTeacher { get; set; } 

    }
}