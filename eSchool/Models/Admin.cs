using Microsoft.AspNet.Identity;
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

    public class Admin : ApplicationUser
    {
        [Column("short_name")]
        public string ShortName { get; set; }

        
    }
}