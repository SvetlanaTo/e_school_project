using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class TeacherToSubjectDTOForStudentAndParent
    {
        [JsonProperty("ID", Order = -1)]
        public int Id { get; set; }

        public virtual TeacherDTOForStudentAndParent Teacher { get; set; }

        public virtual Subject Subject { get; set; }

        public DateTime StartedTeaching { get; set; } 

        public DateTime? StoppedTeaching { get; set; }
    }
}