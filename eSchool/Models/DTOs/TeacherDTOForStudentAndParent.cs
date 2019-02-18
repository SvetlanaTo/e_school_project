using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace eSchool.Models.DTOs
{
    public class TeacherDTOForStudentAndParent 
    {
        [JsonProperty("ID", Order = -2)]
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Genders Gender { get; set; }

        [JsonProperty(Order = 1)]
        public bool IsStillWorking { get; set; } 
    }
}