using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class ReportCardDTOItem
    {
        public ReportCardDTOItem()
        {
            FirstSemesterMarks = new List<int>();
            SecondSemesterMarks = new List<int>();
        }

        public string Subject { get; set; } //id, name
        public string Teacher { get; set; } //id, lastname firstname 

        public IList<int> FirstSemesterMarks { get; set; }
        [Required]
        public double? FirstSemesterAverageMark { get; set; }

        public IList<int> SecondSemesterMarks { get; set; }
        [Required] 
        public double? SecondSemesterAverageMark { get; set; }

    }
}