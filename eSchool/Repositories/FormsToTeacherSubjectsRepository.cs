using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class FormsToTeacherSubjectsRepository : GenericRepository<FormToTeacherSubject>, IFormsToTeacherSubjectsRepository    
    {
        public FormsToTeacherSubjectsRepository(DbContext db) : base(db) { }

        public IEnumerable<FormToTeacherSubject> GetAllActive()
        {
            return Get(x => x.Stopped == null); 
        }

        public IEnumerable<FormToTeacherSubject> GetAllActiveByTSId(int teacherToSubjectId)
        {
            return Get(x => x.TeacherToSubject.Id == teacherToSubjectId && x.Stopped == null); 
        }

        public IEnumerable<FormToTeacherSubject> GetAllByFormId(int formId)
        {
            return Get(x => x.Form.Id == formId && x.Stopped == null);  
        }

        public IEnumerable<FormToTeacherSubject> GetAllByTeacherId(string teacherId)
        {
            return Get(x => x.TeacherToSubject.Teacher.Id == teacherId); 
        }

        public IEnumerable<FormToTeacherSubject> GetAllByTeacherSubjectId(int tsId)
        {
            return Get(x => x.TeacherToSubject.Id == tsId);
        }

        public FormToTeacherSubject GetByFormIdAndSubjectId(int formId, int subjectId) 
        {
            return Get(x => x.Form.Id == formId && x.TeacherToSubject.Subject.Id == subjectId && x.Stopped == null).FirstOrDefault(); 
        }

        public FormToTeacherSubject GetByFormIdAndTeacherId(int formId, string teacherId)
        {
            return Get(x => x.Form.Id == formId && x.TeacherToSubject.Teacher.Id == teacherId && x.Stopped == null).FirstOrDefault(); 
        }

        public FormToTeacherSubject GetByFormIdAndTeacherSubjectId(int formId, int teacherToSubjectId)
        {
            return Get(x => x.Form.Id == formId && x.TeacherToSubject.Id == teacherToSubjectId).FirstOrDefault(); 
        }

        public IEnumerable<FormToTeacherSubject> GetByFormIdOnlyActive(int formId)
        {
            return Get(x => x.Form.Id == formId && x.Stopped == null); 
        }

        public FormToTeacherSubject GetDuplicate(int formId, int teacherToSubjectId)
        {
            return Get(x => x.Form.Id == formId && x.TeacherToSubject.Id == teacherToSubjectId && x.Stopped == null).FirstOrDefault();
        }
    }
}