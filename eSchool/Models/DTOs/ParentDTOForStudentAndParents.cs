using Newtonsoft.Json;

namespace eSchool.Models.DTOs
{
    public class ParentDTOForStudentAndParents
    {
        [JsonProperty("ID", Order = -2)]
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }


    }
}