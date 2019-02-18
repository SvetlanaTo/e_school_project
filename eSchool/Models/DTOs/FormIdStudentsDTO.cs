using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class FormIdStudentsDTO
    {
        public FormIdStudentsDTO()
        {
            Students = new List<FormStudentDTO>();
        }

        [JsonProperty("ID", Order = -1)]
        public int Id { get; set; }

        public int Grade { get; set; }

        public string Tag { get; set; }

        public DateTime Started { get; set; }

        public string AttendingTeacher { get; set; }

        public int NumberOfStudents { get; set; }

        public IList<FormStudentDTO> Students { get; set; }
    }
}