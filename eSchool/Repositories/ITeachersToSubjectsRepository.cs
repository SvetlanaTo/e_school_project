using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface ITeachersToSubjectsRepository : IGenericRepository<TeacherToSubject>
    {
        TeacherToSubject GetByTeacherIdAndSubjectId(string teacherId, int subjectId);
        IEnumerable<TeacherToSubject> GetOngoingEngagementsBySubjectId(int subjectId);
        IEnumerable<TeacherToSubject> GetByTeacherId(string teacherId);
        IEnumerable<TeacherToSubject> GetStoppedEngagementsBySubjectId(int subjectId);
        IEnumerable<TeacherToSubject> GetActiveTeachersByGrade(int grade);
        IEnumerable<TeacherToSubject> GetTeachersForSubjectByDatePeriod(int subjectId, DateTime startDate, DateTime endDate);
        IEnumerable<TeacherToSubject> GetOngoingEngagementsByTeacherId(string teacherId);
        IEnumerable<TeacherToSubject> GetStoppedEngagementsByTeacherId(string teacherId);
    }
}
