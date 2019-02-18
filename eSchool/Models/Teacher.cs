using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Models
{
    public enum Genders { MALE, FEMALE }

    public class Teacher : ApplicationUser
    {
        public Teacher()
        {
            //NASTAVNIKOVI PREDMETI
            TeachersSubjects = new List<TeacherToSubject>();
        }

        [Column("gender")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Genders Gender { get; set; } //TODO reports

        [Column("is_active")]
        public bool IsStillWorking { get; set; } 

        //nastavnik moze, a i ne mora da bude razredni staresina odeljenja
        [Column("form_attending")]
        [JsonIgnore] //problem rekurzije
        public virtual Form FormAttending { get; set; }

        //jedan nastavnik ima vise kombinacija u tabeli TeacherToSubject koje mu pripadaju
        //tj. jedan nastavnik moze da predaje vise predmeta
        //NASTAVNIKOVI PREDMETI
        [JsonIgnore]
        public virtual ICollection<TeacherToSubject> TeachersSubjects { get; set; }




    }
}