using Newtonsoft.Json;

namespace eSchool.Models.DTOs
{
    public class ParentDTOForTeacher : ParentDTOForStudentAndParents
    {
        [JsonProperty(Order = 1)]
        public string Jmbg { get; set; } 
        public string MobilePhone { get; set; }
    }
}