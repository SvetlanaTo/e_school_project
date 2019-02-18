using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Repositories
{
    public class MarksRepository : GenericRepository<Mark>, IMarksRepository      
    {
        public MarksRepository(DbContext db) : base(db) { }

        public IEnumerable<Mark> GetAllByTSId(int teacherToSubjectId)
        {
            return Get(x => x.FormToTeacherSubject.TeacherToSubject.Id == teacherToSubjectId); 
        }

        public IEnumerable<Mark> GetAllMarksByStudentId(string studentId)
        {
            return Get(x => x.Student.Id == studentId);
        }

        public IEnumerable<Mark> GetAllMarksByTeacherId(string teacherId)
        {
            return Get(x => x.FormToTeacherSubject.TeacherToSubject.Teacher.Id == teacherId);  
        }

        public IEnumerable<Mark> GetByFormId(int formId)
        {
            return Get(x => x.Student.Form.Id == formId); 
        }

        public IEnumerable<Mark> GetByFTSId(int ftsId)
        {
            return Get(x => x.FormToTeacherSubject.Id == ftsId); 
        }

        public IEnumerable<Mark> GetByFTSIdAndStudentId(int ftsId, string studentId)
        {
            return Get(x => x.FormToTeacherSubject.Id == ftsId && x.Student.Id == studentId); 
        }

        public Mark GetFirstMarkByActiveSubject(int subjectId)
        {
            return Get(x => x.FormToTeacherSubject.Stopped == null && x.FormToTeacherSubject.TeacherToSubject.StoppedTeaching == null 
            && x.FormToTeacherSubject.TeacherToSubject.Subject.Id == subjectId).FirstOrDefault();
        }

        
    }
}