using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Models.DTOs
{
    public class ReportCardDTO
    {
        public ReportCardDTO()
        {
            Classes = new List<ReportCardDTOItem>(); 
        }

        public int SchoolYear { get; set; } //year started
        public string StudentId { get; set; }
        public string Student { get; set; } //ime i prezime
        public string Form { get; set; } //5-1
        public string AttendingTeacher { get; set; } //id, ime i prezime
        public string Parent { get; set; }

        public IList<ReportCardDTOItem> Classes { get; set; }
    }
}