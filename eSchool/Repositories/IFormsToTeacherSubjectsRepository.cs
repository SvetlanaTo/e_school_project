using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IFormsToTeacherSubjectsRepository : IGenericRepository<FormToTeacherSubject>
    {
        FormToTeacherSubject GetDuplicate(int formId, int teacherToSubjectId);
        IEnumerable<FormToTeacherSubject> GetAllByTeacherId(string teacherId);
        IEnumerable<FormToTeacherSubject> GetAllByTeacherSubjectId(int tsId);
        FormToTeacherSubject GetByFormIdAndSubjectId(int formId, int subjectId);
        FormToTeacherSubject GetByFormIdAndTeacherId(int formId, string teacherId);
        FormToTeacherSubject GetByFormIdAndTeacherSubjectId(int formId, int teacherToSubjectId);
        IEnumerable<FormToTeacherSubject> GetByFormIdOnlyActive(int formId);
        IEnumerable<FormToTeacherSubject> GetAllActive();
        IEnumerable<FormToTeacherSubject> GetAllByFormId(int formId);
        IEnumerable<FormToTeacherSubject> GetAllActiveByTSId(int teacherToSubjectId); 
    }
}
