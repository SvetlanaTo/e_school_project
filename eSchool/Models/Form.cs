using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
    //ODELJENJE
    [Table("forms")]
    public class Form
    {
        public Form()
        {
            Students = new List<Student>();
            FormsTeachersToSubjects = new List<FormToTeacherSubject>();
        }

        [Column("id")]
        [JsonProperty("ID")]
        public int Id { get; set; }

        //RAZRED
        [Column("grade")]
        public int Grade { get; set; }

        //OZNAKA ODELJENJA
        [Column("tag")]     
        public string Tag { get; set; }

        //The Class of
        [Column("started")]
        public DateTime Started { get; set; }

        //RAZREDNI STARESINA
        //odeljenje ima tacno jednog nastavnika koji je razredni staresina
        [Column("attending_teacher_id")]
        public virtual Teacher AttendingTeacher { get; set; } 

        //jedno odeljenje ima vise ucenika koji mu pripadaju
        [JsonIgnore]
        public virtual ICollection<Student> Students { get; set; }

        //jedno odeljenje MOZE da slusa vise predmeta kod vise nastavnika 
        //(pohadja razlicite kombinacije nastavnik-predmet)
        [JsonIgnore]
        public virtual ICollection<FormToTeacherSubject> FormsTeachersToSubjects { get; set; }


    }
}