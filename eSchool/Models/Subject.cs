using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
   

    [Table("subjects")]
    public class Subject
    {
        public Subject()
        {
            //NASTAVNICI PREDMETA
            SubjectsTeachers = new List<TeacherToSubject>();
        }

        [Column("id")]
        [JsonProperty("ID")]
        public int Id { get; set; }

        [Column("subject_name")]
        [Display(Name = "Subject name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        //RAZRED
        [Column("grade")]
        [Display(Name = "Grade")]
        [Range(1, 8, ErrorMessage = "The {0} must be between {1} and {2}.")]
        public int Grade { get; set; } 

        [Column("classes_per_week")]
        [Display(Name = "Number of classes per week")]
        public int NumberOfClassesPerWeek { get; set; }

        //jedan predmet ima vise kombinacija u tabeli TeacherToSubject koje mu pripadaju
        //tj. jedan predmet moze da predaje vise nastavnika
        [JsonIgnore]
        public virtual ICollection<TeacherToSubject> SubjectsTeachers { get; set; }
    }
}