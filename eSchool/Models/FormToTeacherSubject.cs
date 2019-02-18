using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
    [Table("forms_teachers_subjects")]
    public class FormToTeacherSubject
    {
        public FormToTeacherSubject()
        {
            //OCENE KOJE JE DAO NASTAVNIK IZ PREDMETA KOJI PREDAJE ODELJENJU U KOM JE UCENIK KOJI DOBIJA TE OCENE
            Marks = new List<Mark>();
        }

        [Column("id")]
        [JsonProperty("ID")]
        public int Id { get; set; } 

        //odredjena kombinacija odeljenje-nastavnik-predmet pripada odredjenom odeljenju
        [Column("form_id")]
        public virtual Form Form { get; set; }

        //odredjena kombinacija odeljenje-nastavnik-predmet pripada odredjenoj kombinaciji nastavnik-predmet
        [Column("teacher_subject_id")] 
        public virtual TeacherToSubject TeacherToSubject { get; set; }

        [Column("started")]
        [Required]
        public DateTime Started { get; set; } //= DateTime.UtcNow;

        [Column("stopped")]
        public DateTime? Stopped { get; set; }  

        //jedna kombinacija odeljenje-nastavnik-predmet se moze nalaziti na vise ocena
        [JsonIgnore]
        public virtual ICollection<Mark> Marks { get; set; }

    }
}