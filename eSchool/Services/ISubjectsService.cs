using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface ISubjectsService
    {
        IEnumerable<Subject> GetAll();
        Subject GetByID(int id);
        Subject Update(int id, PutSubjectDTO updated);
        Subject Create(PostSubjectDTO newSubject);
        Subject Delete(int id);
        IEnumerable<Subject> GetSortedSubjectsByFirstLetter(string firstLetter);
        IEnumerable<Subject> GetSortedSubjectsByGrade(int grade);

    }
}
