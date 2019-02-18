using Newtonsoft.Json;

namespace eSchool.Models.DTOs
{
    public class TeacherDTOForTeacher : TeacherDTOForStudentAndParent
    {
        [JsonProperty(Order = 1)] 
        public string Jmbg { get; set; }

    }
}