using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IFormsRepository : IGenericRepository<Form>
    {
        Form GetDuplicate(int grade, string tag, int year);
        IEnumerable<Form> GetAllByGradeByYear(int grade, int year);
        Form GetByAttendingTeacherId(string teacherid);

    }
}
