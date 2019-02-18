using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
    public enum Semesters { FIRST_SEMESTER, SECOND_SEMESTER }

    [Table("marks")]
    public class Mark
    {
        public Mark()
        {
        }

        [Column("id")]
        [JsonProperty("ID")]
        public int Id { get; set; }

        [Column("mark_value")]
        [Display(Name = "Mark value")]
        [Range(1, 5, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int MarkValue { get; set; }

        [Column("semester")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Semesters Semester { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } //= new DateTime();

        //jednu ocenu dodeljuje tacno jedan nastavnik koji predaje jedan predmet jednom odeljenju u kom je student cija je ocena
        [Column("form_teacher_student_id")]
        public virtual FormToTeacherSubject FormToTeacherSubject { get; set; }

        //jedna ocena se dodeljuje tacno jednom studentu
        [Column("student_id")]
        public virtual Student Student { get; set; }



    }
}