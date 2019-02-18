using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IStudentsRepository : IGenericRepository<Student>
    {
        Student GetByUserName(string username);
        Student GetByJmbg(string jmbg);
        IEnumerable<Student> GetAllByLastName(string lastName);
    }
}
