using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface ITeachersToSubjectsService
    {
        IEnumerable<TeacherToSubject> GetAll();
        TeacherToSubject GetByID(int id);
        TeacherToSubject GetTeacherToSubjectByTeacherIdAndSubjectId(string teacherId, int subjectId);
        TeacherToSubject GetActiveTeacherToSubjectByTeacherIdAndSubjectId(string teacherId, int subjectId);
        TeacherToSubjectDTOForAdmin Create(string teacherId, int subjectId);
        TeacherToSubjectDTOForAdmin UpdateStartedTeaching(int id, DateTime updated);
        TeacherToSubjectDTOForAdmin UpdateStoppedTeaching(int id, DateTime updated);
        IEnumerable<TeacherToSubject> GetSubjectsByTeacherId(string teacherId);
        IEnumerable<TeacherToSubject> GetTeachersBySubjectId(int subjectId);
        IEnumerable<TeacherToSubject> UpdateStoppedTeachingToNowForAllSubjectsByTeacherId(string teacherId);
        IEnumerable<TeacherToSubject> UpdateStoppedTeachingToNowForAllTeachersBySubjectId(int subjectId);
        TeacherToSubjectDTOForAdmin PutStoppedTeachingNowByTSId(int id);
        TeacherToSubject Delete(int id);
        IEnumerable<TeacherToSubject> GetActiveOrInactiveTeachersBySubjectId(int subjectId, string active);
        IEnumerable<TeacherToSubject> GetActiveTeachersForGrade(int grade);
        IEnumerable<TeacherToSubject> GetTeachersForSubjectByDate(int subjectId, DateTime startDate, DateTime endDate);
        IEnumerable<TeacherToSubject> GetSubjectsByActiveOrInactiveTeacherId(string teacherId, string active);
        SubjectTeacherSubjectDTOItems GetTeacherDTOListBySubjectId(int subjectId);
        TeacherTeacherSubjectDTOItems GetSubjectDTOListByTeacherId(string teacherId);

    }
}
