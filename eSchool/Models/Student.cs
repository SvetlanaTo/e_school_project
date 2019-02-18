using eSchool.Support;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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
    public class Student : ApplicationUser
    {
        public Student()
        {
            Marks = new List<Mark>();
        }

        [Column("day_of_birth")]
        [Required]
        public DateTime DayOfBirth { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        [Column("is_active")]
        [Required]
        public bool IsActive { get; set; }

        //jedan ucenik pripada tacno jednom roditelju.
        [Column("parent_id")]
        public virtual Parent Parent { get; set; }

        //jedan ucenik pripada tacno jednom odeljenju
        [Column("form_id")]
        public virtual Form Form { get; set; }

        //jedan student moze imati vise ocena 
        [JsonIgnore]
        public virtual ICollection<Mark> Marks { get; set; }
       
    }
}