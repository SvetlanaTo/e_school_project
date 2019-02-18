using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface ISubjectsRepository : IGenericRepository<Subject>
    {
        Subject Duplicate(string name, int grade, int numOfClassesPerWeek);
        IEnumerable<Subject> GetAllByFirstLetter(string firstLetter);
        IEnumerable<Subject> GetAllByGrade(int grade); 
    }
}
