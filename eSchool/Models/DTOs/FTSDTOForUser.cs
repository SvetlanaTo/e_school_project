using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class FTSDTOForUser
    {

        [JsonProperty("ID", Order = -1)] 
        public int Id { get; set; }

        //odredjena kombinacija odeljenje-nastavnik-predmet pripada odredjenom odeljenju
        public virtual FormDTOForStudentAndParents Form { get; set; }

        //odredjena kombinacija odeljenje-nastavnik-predmet pripada odredjenoj kombinaciji nastavnik-predmet
        public virtual TeacherToSubjectDTOForStudentAndParent TeacherToSubject { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Stopped { get; set; } 
    }
}