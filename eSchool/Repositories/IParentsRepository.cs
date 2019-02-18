using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IParentsRepository : IGenericRepository<Parent>
    {
        Parent GetByUserName(string username);
        Parent GetByJmbg(string jmbg);
    }
}
