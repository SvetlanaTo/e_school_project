using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class SubjectsRepository : GenericRepository<Subject>, ISubjectsRepository  
    {
        public SubjectsRepository(DbContext db) : base(db) { }

        public Subject Duplicate(string name, int grade, int numOfClassesPerWeek)
        {
            return Get(x => x.Name == name && x.Grade == grade
            && x.NumberOfClassesPerWeek == numOfClassesPerWeek).FirstOrDefault(); 
        }

        public IEnumerable<Subject> GetAllByFirstLetter(string firstLetter)
        {
            return Get(x => x.Name.ToLower().StartsWith(firstLetter.ToLower())); 
        }

        public IEnumerable<Subject> GetAllByGrade(int grade)
        {
            return Get(x => x.Grade == grade);
        }
    }
}