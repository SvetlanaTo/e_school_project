using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class FormsRepository : GenericRepository<Form>, IFormsRepository   
    {
        public FormsRepository(DbContext db) : base(db) { }

        public IEnumerable<Form> GetAllByGradeByYear(int grade, int year)
        {
            return Get(x => x.Grade == grade && x.Started.Year == year); 
        }

        public Form GetByAttendingTeacherId(string teacherid)
        {
            return Get(x => x.AttendingTeacher.Id == teacherid).FirstOrDefault(); 
        }

        public Form GetDuplicate(int grade, string tag, int year)
        {
            return Get(x => x.Grade == grade && x.Tag == tag && x.Started.Year == year).FirstOrDefault(); 
        }
    }
}