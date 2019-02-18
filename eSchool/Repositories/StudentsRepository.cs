using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class StudentsRepository : GenericRepository<Student>, IStudentsRepository 
    {
        public StudentsRepository(DbContext db) : base(db) { } 

        public Student GetByUserName(string username)
        {
            return Get(x => x.UserName.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public Student GetByJmbg(string jmbg)
        {
            return Get(x => x.Jmbg == jmbg).FirstOrDefault();
        }

        public IEnumerable<Student> GetAllByLastName(string lastName)
        {
            return Get(x => x.LastName.ToLower() == lastName.ToLower()); 
        }
    }

}