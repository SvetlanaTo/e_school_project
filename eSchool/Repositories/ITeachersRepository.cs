using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface ITeachersRepository : IGenericRepository<Teacher>
    {
        Teacher GetByUserName(string username);
        Teacher GetByJmbg(string jmbg);
        IEnumerable<Teacher> GetAllStillWorkingTeachers();
        Teacher GetByLastName(string teacherLastName);
        IEnumerable<Teacher> GetWorkingTeachersByFirstThreeLetters(string startsWith);
    }
}
