using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IMarksRepository : IGenericRepository<Mark> 
    {

        IEnumerable<Mark> GetAllMarksByStudentId(string studentId);
        IEnumerable<Mark> GetAllByTSId(int teacherToSubjectId);
        IEnumerable<Mark> GetAllMarksByTeacherId(string teacherId);
        IEnumerable<Mark> GetByFTSId(int ftsId);
        IEnumerable<Mark> GetByFTSIdAndStudentId(int ftsId, string studentId);
        IEnumerable<Mark> GetByFormId(int formId);
        Mark GetFirstMarkByActiveSubject(int subjectId);
        
    }
}
