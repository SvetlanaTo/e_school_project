using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface ITeachersService
    {
        IEnumerable<Teacher> GetAll();
        Teacher GetById(string id);
        Teacher GetByUserName(string userName);
        IEnumerable<TeacherDTOForStudentAndParent> GetAllForStudentAndParent();
        IEnumerable<TeacherDTOForTeacher> GetAllForTeacher();
        IEnumerable<TeacherDTOForAdmin> GetAllForAdmin();
        Task<TeacherDTOForAdmin> Update(string id, PutTeacherDTO updated);
        Teacher Delete(string id);
        IList<Teacher> GetByLastNameSorted(string lastName);
        IList<Teacher> GetWorkingTeachersByLastNameSorted(string lastName);
        TeacherDTOForAdmin StoppedWorkingAndEndedAllEngagementsInTSAndFTSByTeacherId(string id);
        StudentTeacherDTOItems GetTeachersByStudentUserName(string studentUserName);
        StudentTeacherDTOItems GetTeachersByStudentUserNameForParent(string studentUserName, string parentId);
        IEnumerable<TeacherDTOItem> GetWorkingTeachersByFirstThreeLetters(string startsWith);

    }
}
