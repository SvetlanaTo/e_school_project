using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class ParentsRepository : GenericRepository<Parent>, IParentsRepository
    {
        public ParentsRepository(DbContext db) : base(db) { }

        public Parent GetByUserName(string username)
        {
            return Get(x => x.UserName.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public Parent GetByJmbg(string jmbg)
        {
            return Get(x => x.Jmbg == jmbg).FirstOrDefault();
        }
    }
}