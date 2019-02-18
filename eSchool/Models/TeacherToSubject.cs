using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
    [Table("teachers_subjects")]
    public class TeacherToSubject
    {
        public TeacherToSubject()
        {
            TeacherSubjectForms = new List<FormToTeacherSubject>();
        }

        [Column("id")]
        [JsonProperty("ID")]
        public int Id { get; set; }  

        //jedna kombinacija (nastavnik-predmet-datumOdKadaPredaje) pripada tacno jednom nastavniku
        [Column("teacher_id")]
        [Required]
        public virtual Teacher Teacher { get; set; }

        //jedna kombinacija (nastavnik-predmet-datumOdKadaPredaje) pripada tacno jednom predmetu
        [Column("subject_id")]
        [Required]
        public virtual Subject Subject { get; set; }

        [Column("started_teaching")]
        [Required]
        public DateTime StartedTeaching { get; set; } //= DateTime.UtcNow;

        [Column("stopped_teaching")]
        public DateTime? StoppedTeaching { get; set; }

        //jedna kombinacija nastavnik-predmet MOZE da vazi za vise odeljenja
        //vise odeljenja MOZE da slusa isti predmet kod istog nastavnika
        [JsonIgnore]
        public virtual ICollection<FormToTeacherSubject> TeacherSubjectForms { get; set; }
    }
}