using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class TeachersToSubjectsRepository : GenericRepository<TeacherToSubject>, ITeachersToSubjectsRepository      
    { 
        public TeachersToSubjectsRepository(DbContext db) : base(db) { }

        public IEnumerable<TeacherToSubject> GetActiveTeachersByGrade(int grade)
        {
            return Get(x => x.Subject.Grade == grade && (x.StoppedTeaching == null || x.StoppedTeaching > DateTime.UtcNow));  
        }

        public IEnumerable<TeacherToSubject> GetByTeacherId(string teacherId)
        {
            return Get(x => x.Teacher.Id == teacherId); 
        }

        public TeacherToSubject GetByTeacherIdAndSubjectId(string teacherId, int subjectId) 
        {
            return Get(x => x.Teacher.Id == teacherId && x.Subject.Id == subjectId).FirstOrDefault(); 
        }

        public IEnumerable<TeacherToSubject> GetOngoingEngagementsBySubjectId(int subjectId)
        {
            return Get(x => x.Subject.Id == subjectId && (x.StoppedTeaching == null || x.StoppedTeaching > DateTime.UtcNow)); 
        }

        public IEnumerable<TeacherToSubject> GetOngoingEngagementsByTeacherId(string teacherId)
        {
            return Get(x => x.Teacher.Id == teacherId && (x.StoppedTeaching == null || x.StoppedTeaching > DateTime.UtcNow)); 
        }

        public IEnumerable<TeacherToSubject> GetStoppedEngagementsBySubjectId(int subjectId)
        {
            return Get(x => x.Subject.Id == subjectId && (x.StoppedTeaching != null || x.StoppedTeaching < DateTime.UtcNow)); 
        }

        public IEnumerable<TeacherToSubject> GetStoppedEngagementsByTeacherId(string teacherId)
        {
            return Get(x => x.Teacher.Id == teacherId && (x.StoppedTeaching != null || x.StoppedTeaching < DateTime.UtcNow)); 
        }

        public IEnumerable<TeacherToSubject> GetTeachersForSubjectByDatePeriod(int subjectId, DateTime startDate, DateTime endDate)
        {
            return Get(x => x.Subject.Id == subjectId && (x.StartedTeaching >= startDate && x.StartedTeaching <= endDate));  
        }
    }
}