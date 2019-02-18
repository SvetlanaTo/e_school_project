using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IStudentsService
    {
        IEnumerable<Student> GetAll();
        Student GetByID(string id);
        Student GetByUserName(string userName);
        IEnumerable<StudentDTOForAdmin> GetAllForAdmin();
        IEnumerable<StudentDTOForTeacher> GetAllForTeacher();
        IEnumerable<StudentDTOForStudentAndParent> GetAllForStudentFromStudentForm(string userId);
        IEnumerable<StudentDTOForStudentAndParent> GetAllForParentFromStudentsForms(string userId);
        Task<StudentDTOForAdmin> Update(string id, PutStudentDTO updated);
        Student Delete(string id);
        StudentDTOForAdmin UpdateStudentWithImage(string id, string localFileName);
        StudentDTOForAdmin UpdateStudentParent(string id, string parentId);
        IEnumerable<Student> GetStudentsAssignedToParentId(string parentId);
        StudentDTOForAdmin UpdateStudentForm(string id, int formId);
        IEnumerable<Student> GetStudentsByFormId(int formId);
        IEnumerable<FormStudentDTO> GetStudentsByLastNameSorted(string lastName);

    }
}
