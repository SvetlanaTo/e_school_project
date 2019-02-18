using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class FormStudentDTO
    {
        [JsonProperty("ID")]
        public string Id { get; set; }
        public string Student { get; set; }

    }
}