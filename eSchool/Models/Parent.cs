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
    public class Parent : ApplicationUser
    {
        public Parent()
        {
            Students = new List<Student>();
        }

        [Column("mobile_phone")]
        public string MobilePhone { get; set; }

        //jedan roditelj moze da ima vise ucenika (dece) koji mu pripadaju
        [JsonIgnore]
        public virtual ICollection<Student> Students { get; set; }


    }
}