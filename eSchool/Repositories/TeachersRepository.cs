using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class TeachersRepository : GenericRepository<Teacher>, ITeachersRepository  
    {
        public TeachersRepository(DbContext db) : base(db) { }

        public Teacher GetByUserName(string username)
        {
            return Get(x => x.UserName.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public Teacher GetByJmbg(string jmbg)
        {
            return Get(x => x.Jmbg == jmbg).FirstOrDefault();
        }

        public IEnumerable<Teacher> GetAllStillWorkingTeachers()
        {
            return Get(x => x.IsStillWorking == true);
        }

        public Teacher GetByLastName(string teacherLastName)
        {
            return Get(x => x.LastName.ToLower() == teacherLastName.ToLower()).FirstOrDefault(); 
        }

        public IEnumerable<Teacher> GetWorkingTeachersByFirstThreeLetters(string startsWith)
        {
            return Get(x => x.IsStillWorking == true 
            && (x.FirstName.Substring(0, 3).ToLower().Equals(startsWith.ToLower()) || x.LastName.Substring(0, 3).ToLower().Equals(startsWith.ToLower())));
        }
    }
}